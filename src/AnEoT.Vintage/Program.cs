using System.Text.Encodings.Web;
using System.Text.Unicode;
using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.Helpers.Custom;
using AnEoT.Vintage.Models;
using AspNetStatic;
using AspNetStatic.Optimizer;
using Markdig;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.WebEncoders;
using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage;

/// <summary>
/// 程序启动的入口类
/// </summary>
public class Program
{
    private static readonly string[] defaultFileNames = ["README.md", "index.html", "index.htm"];
    internal static bool ConvertWebP;

    /// <summary>
    /// 入口点方法
    /// </summary>
    /// <param name="args">启动时传递的参数</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        #region 第一步：读取应用程序配置

        // 确定是否启用 WebP 图像转换功能
        {
            if (!bool.TryParse(builder.Configuration["ConvertWebP"], out bool convertWebP))
            {
                convertWebP = true;
            }
            ConvertWebP = convertWebP;
        }

        // 设置网站基 Uri
        string baseUri;

        {
            string[]? hostUrls = builder.Configuration["urls"]?.Split(';');
            string? rssBaseUriInConfig = builder.Configuration["BaseUri"];

            if (string.IsNullOrWhiteSpace(rssBaseUriInConfig))
            {
                baseUri = hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
                        ?? hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidOperationException("现在无法获取到基 Uri");
            }
            else
            {
                baseUri = rssBaseUriInConfig;
            }
        }
        //CurrentBaseUri = new Uri(baseUri, UriKind.Absolute);

        #region 静态页面
        // 设置静态页面的导出位置
        string staticWebSiteOutputPath;

        {
            string? outputPathInConfig = builder.Configuration["StaticWebSiteOutputPath"];
            if (!string.IsNullOrWhiteSpace(outputPathInConfig))
            {
                staticWebSiteOutputPath = outputPathInConfig;
            }
            else
            {
                string defaultPath = Path.Combine(builder.Environment.ContentRootPath, "StaticWebSite");
                Directory.CreateDirectory(defaultPath);
                staticWebSiteOutputPath = defaultPath;
            }
        }

        // 确定是否生成静态网页
        bool generateStaticWebSite = args.HasExitWhenDoneArg();
        #endregion

        #region Feed
        // 确定是否生成完整源 （包含全部文章）
        if (!bool.TryParse(builder.Configuration["FeedIncludeAllArticles"], out bool feedIncludeAllArticles))
        {
            feedIncludeAllArticles = true;
        }

        // 确定生成的源是否包含样式
        if (!bool.TryParse(builder.Configuration["FeedAddCssStyle"], out bool feedAddCssStyle))
        {
            feedAddCssStyle = true;
        }
        #endregion
        #endregion

        #region 第二步：向依赖注入容器添加服务

        builder.Services.AddSingleton<VolumeDirectoryOrderComparer>();
        builder.Services.AddSingleton<ArticleFileOrderComparer>();
        builder.Services.AddSingleton<VolumeInfoHelper>();
        builder.Services.AddSingleton<BaseUriHelper>();

        builder.Services.AddTransient<TileHelper>();

        if (generateStaticWebSite)
        {
            // 添加静态网站生成服务
            builder.Services.AddSingleton<IStaticResourcesInfoProvider>(provider =>
            {
                return StaticWebSiteHelper.GetStaticResourcesInfo(builder.Environment.WebRootPath, ConvertWebP);
            });

            if (ConvertWebP)
            {
                builder.Services.AddSingleton<IBinOptimizer, WebPContentConverter>();
            }
        }
#if DEBUG
        else
        {
            ConvertWebP = false;
        }
#endif

        // 添加 Markdown 解析服务
        builder.Services.AddMarkdown(config =>
        {
            // 设置 Markdown 中间件的工作文件夹
            config.AddMarkdownProcessingFolder("/");
            config.ConfigureMarkdigPipeline = builder =>
            {
                builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                    .UseAdvancedExtensions()
                    .UseListExtras()
                    .UseEmojiAndSmiley(true)
                    .UseYamlFrontMatter();
            };

            config.MarkdownParserFactory = new CustomMarkdownParserFactory(ConvertWebP);
        });

        builder.Services.AddControllersWithViews()
            .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);

        // 注入 IUrlHelper 服务
        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddScoped(provider =>
            {
                return provider.GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(provider.GetRequiredService<IActionContextAccessor>().ActionContext!);
            });

        builder.Services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });

        WebApplication app = builder.Build();
        #endregion

        #region 第三步：配置 HTTP 请求管道 + 额外工作
        // 配置 Uri 重写
        RewriteOptions rewriteOptions = new RewriteOptions()
            .Add(context =>
            {
                HttpRequest request = context.HttpContext.Request;

                if (request.Path.HasValue)
                {
                    if (request.Path.Value == "/")
                    {
                        request.Path = "/Home/Index";
                    }
                    else if (request.Path.Value.Contains("index.html", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Path = request.Path.Value.Replace("index.html", "README.md");
                    }
                    else
                    {
                        request.Path = request.Path.Value.Replace(".html", ".md");
                    }
                    context.Result = RuleResult.SkipRemainingRules;
                }
            });

        // 启用 Uri 重写
        app.UseRewriter(rewriteOptions);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error/HandleError");
        }
        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        // 让 Markdown 中间件能够自动获取到期刊页面的 README.md 文件
        app.UseDefaultFiles(new DefaultFilesOptions()
        {
            DefaultFileNames = defaultFileNames
        });
       
        app.UseAuthorization();

        app.UseMarkdown();

        app.UseStaticFiles();

        app.UseRouting();
        app.MapDefaultControllerRoute();

        FakeAdHelper.PrepareData(app.Environment.WebRootPath);

        if (feedIncludeAllArticles)
        {
            FeedGenerationHelper.GenerateFeed(baseUri, app.Environment.WebRootPath, includeAllArticles: true, addCssStyle: feedAddCssStyle, rss20FileName: "rss_full.xml", atomFileName: "atom_full.xml");
            FeedGenerationHelper.GenerateFeed(baseUri, app.Environment.WebRootPath, includeAllArticles: true, addCssStyle: feedAddCssStyle, generateDigest: true, rss20FileName: "rss_full_digest.xml", atomFileName: "atom_full_digest.xml");
        }
        FeedGenerationHelper.GenerateFeed(baseUri, app.Environment.WebRootPath, addCssStyle: feedAddCssStyle);

        FeedGenerationHelper.GenerateFeed(baseUri, app.Environment.WebRootPath, generateDigest: true, addCssStyle: feedAddCssStyle, rss20FileName: "rss_digest.xml", atomFileName: "atom_digest.xml");

        app.GenerateTileXml().GenerateLatestVolumeInfoJson();

        if (generateStaticWebSite)
        {
            if (Directory.Exists(staticWebSiteOutputPath) != true)
            {
                Directory.CreateDirectory(staticWebSiteOutputPath);
            }

            // 生成静态网页文件
            app.GenerateStaticContent(staticWebSiteOutputPath, exitWhenDone: true);
        }

        app.Run();
        #endregion
    }
}