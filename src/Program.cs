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

            //������ע��������ӷ���
            builder.Services.AddMarkdown(config =>
            {
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
            builder.Services.AddDirectoryBrowser();
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
            builder.Services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped(provider =>
                {
                    return provider.GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(provider.GetRequiredService<IActionContextAccessor>().ActionContext!);
                });
            WebApplication app = builder.Build();

            //���� HTTP ����ܵ�

            RewriteOptions options = new RewriteOptions()
                .Add((context) =>
                {
                    HttpRequest request = context.HttpContext.Request;

                    context.Result = RuleResult.SkipRemainingRules;
                    request.Path = request.Path.Value?.Replace(".html", ".md");
                });

            app.UseRewriter(options);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error/HandleError");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new string[] { "README.md" }
            });

            app.UseMarkdown();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            app.Run();
        }
    }
}