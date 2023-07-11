using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.Helpers.Custom;
using AspNetStatic;
using Markdig;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using SixLabors.ImageSharp;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage
{
    /// <summary>
    /// ���������������
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ��ڵ㷽��
        /// </summary>
        /// <param name="args">����ʱ���ݵĲ���</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            #region ��ȡӦ�ó�������

            //���þ�̬ҳ��ĵ���λ��
            string staticWebSiteOutputPath;

            string? pathInConfig = builder.Configuration["StaticWebSiteOutputPath"];
            if (!string.IsNullOrWhiteSpace(pathInConfig))
            {
                staticWebSiteOutputPath = pathInConfig;
            }
            else
            {
                string defaultPath = Path.Combine(builder.Environment.ContentRootPath, "StaticWebSite");
                Directory.CreateDirectory(defaultPath);
                staticWebSiteOutputPath = defaultPath;
            }

            //ȷ���Ƿ����ɾ�̬��ҳ
            bool generateStaticWebSite = args.HasExitWhenDoneArg();

            //ȷ���Ƿ�����WebPͼ��ת������
            _ = bool.TryParse(builder.Configuration["ConvertWebP"], out bool convertWebP);
            #endregion

            #region ��һ����������ע��������ӷ���

            //���Markdown��������
            builder.Services.AddMarkdown(config =>
            {
                //����Markdown�м���Ĺ����ļ���
                config.AddMarkdownProcessingFolder("/");
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UseAdvancedExtensions()
                        .UseListExtras()
                        .UseEmojiAndSmiley(true)
                        .UseYamlFrontMatter();
                };

                if (generateStaticWebSite)
                {
                    //Ϊ��̬��ҳ���������Markdown��������
                    config.MarkdownParserFactory = new CustomMarkdownParserFactory(convertWebP);
                }
            });
            builder.Services.AddControllersWithViews()
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
            
            //���Swagger API�ĵ�
            builder.Services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AnEoT API",
                    Description = "���ع��ߡ��Ĺ���API"
                });

                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            //ע��IUrlHelper����
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
                //��Ӿ�̬��վ���ɷ���
                StaticPagesInfoProvider provider = StaticWebSiteHelper.GetStaticPagesInfoProviderAndCopyFiles(builder.Environment.WebRootPath, staticWebSiteOutputPath, convertWebP);
                builder.Services.AddSingleton<IStaticPagesInfoProvider>(provider);
            }

            WebApplication app = builder.Build();
            #endregion

            #region �ڶ��������� HTTP ����ܵ�
            RewriteOptions rewriteOptions = new RewriteOptions()
                .Add(context =>
                {
                    HttpRequest request = context.HttpContext.Request;

                    context.Result = RuleResult.SkipRemainingRules;
                    if (request.Path.HasValue && !request.Path.Value.Contains("swagger"))
                    {
                        if (request.Path.Value.Contains("api"))
                        {
                            request.Path = request.Path.Value
                                .Replace(".md", string.Empty)
                                .Replace(".html", string.Empty);
                        }
                        else
                        {
                            request.Path = request.Path.Value.Replace(".html", ".md");
                        }
                    }
                });
            
            //����Uri��д
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

            app.UseSwagger();
            app.UseSwaggerUI();

            //��Markdown�м���ܹ��Զ���ȡ���ڿ�ҳ���README.md�ļ�
            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new string[] { "README.md", "index.html", "index.htm" }
            });
           
            app.UseAuthorization();

            app.UseMarkdown();

            if (convertWebP)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = async context =>
                    {
                        string acceptHeader = context.Context.Request.Headers["Accept"].ToString();

                        if (!acceptHeader.Contains("image/webp")
                            && context.File.Name.EndsWith(".webp")
                            && context.Context.Response.StatusCode != StatusCodes.Status304NotModified
                            && context.File.PhysicalPath is not null)
                        {
                            context.Context.Response.Headers.Remove("Content-Length");

                            using Image image = Image.Load(context.File.PhysicalPath);
                            context.Context.Response.ContentType = "image/jpeg";

                            await image.SaveAsJpegAsync(context.Context.Response.Body);
                        }
                    }
                });
            }
            else
            {
                app.UseStaticFiles();
            }

            app.UseRouting();
            app.MapDefaultControllerRoute();

            if (generateStaticWebSite)
            {
                //���ɾ�̬��ҳ�ļ�
                app.GenerateStaticPages(staticWebSiteOutputPath, exitWhenDone: generateStaticWebSite, dontOptimizeContent: false);
            }

            app.Run();
            #endregion
        }
    }
}