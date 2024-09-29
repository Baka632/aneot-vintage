using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.Helpers.Custom;
using AspNetStatic;
using Markdig;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage;

/// <summary>
/// 程序启动的入口类
/// </summary>
public class Program
{
    private static readonly string[] defaultFileNames = ["README.md", "index.html", "index.htm"];

    /// <summary>
    /// 入口点方法
    /// </summary>
    /// <param name="args">启动时传递的参数</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        #region 第一步：读取应用程序配置
        // 确定是否启用 WebP 图像转换功能
        _ = bool.TryParse(builder.Configuration["ConvertWebP"], out bool convertWebP);

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

        #region RSS
        // 设置 RSS 源的基 Uri
        string rssBaseUri;

        {
            string[]? hostUrls = builder.Configuration["urls"]?.Split(';');
            string? rssBaseUriInConfig = builder.Configuration["RssBaseUri"];

            if (string.IsNullOrWhiteSpace(rssBaseUriInConfig))
            {
                rssBaseUri = hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
                        ?? hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidOperationException("现在无法获取到基 Uri");
            }
            else
            {
                rssBaseUri = rssBaseUriInConfig;
            }
        }

        // 确定是否生成完整的 RSS 源 （包含全部文章）
        _ = bool.TryParse(builder.Configuration["RssIncludeAllArticles"], out bool rssIncludeAllArticles);
        
        // 确定生成的 RSS 源是否包含样式
        _ = bool.TryParse(builder.Configuration["RssAddCssStyle"], out bool rssAddCssStyle);
        #endregion
        #endregion

        #region 第二步：向依赖注入容器添加服务

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

            config.MarkdownParserFactory = new CustomMarkdownParserFactory(convertWebP);
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

        if (generateStaticWebSite)
        {
            // 添加静态网站生成服务
            builder.Services.AddSingleton<IStaticResourcesInfoProvider>(provider =>
            {
                return StaticWebSiteHelper.GetStaticResourcesInfo(builder.Environment.WebRootPath, convertWebP);
            });

            if (convertWebP)
            {
                builder.Services.AddSingleton<IBinOptimizer, WebPContentConverter>();
            }
        }

        WebApplication app = builder.Build();
        #endregion

        #region 第三步：配置 HTTP 请求管道 + 额外工作
        // 配置 Uri 重写
        RewriteOptions rewriteOptions = new RewriteOptions()
            .Add(context =>
            {
                HttpRequest request = context.HttpContext.Request;

                context.Result = RuleResult.SkipRemainingRules;
                if (request.Path.HasValue && !request.Path.Value.Contains("swagger", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.Path.Value.Contains("api", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Path = request.Path.Value
                            .Replace(".md", string.Empty)
                            .Replace(".html", string.Empty);
                    }
                    else
                    {
                        if (request.Path.Value.Contains("index.html", StringComparison.OrdinalIgnoreCase))
                        {
                            request.Path = request.Path.Value.Replace("index.html", "README.md");
                        }
                        else
                        {
                            request.Path = request.Path.Value.Replace(".html", ".md");
                        }
                    }
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

        if (rssIncludeAllArticles)
        {
            FeedGenerationHelper.GenerateFeed(rssBaseUri, app.Environment.WebRootPath, includeAllArticles: true, addCssStyle: rssAddCssStyle, rss20FileName: "rss_full.xml", atomFileName: "atom_full.xml");
            FeedGenerationHelper.GenerateFeed(rssBaseUri, app.Environment.WebRootPath, includeAllArticles: true, generateDigest: true, rss20FileName: "rss_full_digest.xml", atomFileName: "atom_full_digest.xml");
        }
        FeedGenerationHelper.GenerateFeed(rssBaseUri, app.Environment.WebRootPath, addCssStyle: rssAddCssStyle);

        FeedGenerationHelper.GenerateFeed(rssBaseUri, app.Environment.WebRootPath, generateDigest: true, rss20FileName: "rss_digest.xml", atomFileName: "atom_digest.xml");

        TileHelper.GenerateTileXml(rssBaseUri, app.Environment.WebRootPath);

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