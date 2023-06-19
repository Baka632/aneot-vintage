using Markdig;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage
{
    /// <summary>
    /// 程序启动的入口类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 入口点方法
        /// </summary>
        /// <param name="args">启动时传递的参数</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            //第一步：向依赖注入容器添加服务

            //添加Markdown解析服务
            builder.Services.AddMarkdown(config =>
            {
                //设置Markdown中间件的工作文件夹
                config.AddMarkdownProcessingFolder("/");
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UseAdvancedExtensions()
                        .UseListExtras()
                        .UseEmojiAndSmiley(true)
                        .UseYamlFrontMatter();
                };
            });
            builder.Services.AddControllersWithViews()
                .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);
            
            //添加Swagger API文档
            builder.Services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AnEoT API",
                    Description = "《回归线》的公共API"
                });

                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            //注入IUrlHelper服务
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

            //第二步：配置 HTTP 请求管道

            RewriteOptions rewriteOptions = new RewriteOptions()
                .Add((context) =>
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
            
            //启用Uri重写
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

            //让Markdown中间件能够自动获取到期刊页面的README.md文件
            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new string[] { "README.md" }
            });
           
            app.UseAuthorization();

            app.UseMarkdown();
            app.UseStaticFiles();

            app.UseRouting();
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}