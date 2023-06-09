using AnEoT.Vintage.ViewModels.Posts;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Parsers;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using SystemIOFile = System.IO.File;
using YamlDotNet.Core.Events;
using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 文章查看页面的控制器
    /// </summary>
    public partial class PostsController : Controller
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<HomeController> _logger;
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// 构造<see cref="PostsController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public PostsController(IWebHostEnvironment env, ILogger<HomeController> logger)
        {
            environment = env;
            _logger = logger;
        }

        /// <summary>
        /// 查看期刊列表
        /// </summary>
        public IActionResult Index()
        {
            //TODO: 改成依赖注入？
            string path = Path.Combine(environment.WebRootPath, "aneot", "posts");

            if (!Directory.Exists(path))
            {
                _logger.LogCritical("未能找到含有《回归线》内容的文件夹！使用的路径：{path}", path);
                return NotFound();
            }

            DirectoryInfo directoryInfo = new(path);
            IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();

            return base.View(new IndexViewModel(directories));
        }

        /// <summary>
        /// 查看期刊或文章
        /// </summary>
        /// <param name="post">期刊名称</param>
        /// <param name="article">文章名称</param>
        [Route("[controller]/{post}/{article?}")]
        public IActionResult View(string post, string? article)
        {
            if (post is null)
            {
                return BadRequest();
            }

            string path;
            ViewViewModel model = new();

            if (string.IsNullOrWhiteSpace(article))
            {
                //操作：查看指定的期刊
                path = Path.Combine(environment.WebRootPath, "aneot", "posts", post, "README.md");

                if (!SystemIOFile.Exists(path))
                {
                    return NotFound();
                }

                string markdown = SystemIOFile.ReadAllText(path);
                #region Front Matter解析
                MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                    .UseYamlFrontMatter().Build();
                MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
                YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

                if (yamlBlock is not null)
                {
                    string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
                    using StringReader input = new(yaml);

                    Parser yamlParser = new(input);
                    yamlParser.Consume<StreamStart>();
                    yamlParser.Consume<DocumentStart>();

                    IDeserializer yamlDes = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    PostInfo postInfo = yamlDes.Deserialize<PostInfo>(yamlParser);
                    yamlParser.Consume<DocumentEnd>();
                    model.Title = postInfo.Title;
                    model.PostInfo = postInfo;
                }
                #endregion

                model.Markdown = markdown;
            }
            else
            {
                //操作：查看指定的文章
            }

            return base.View(model);
        }
    }
}
