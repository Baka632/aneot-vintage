using AnEoT.Vintage.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;
using AnEoT.Vintage.Models;
using AnEoT.Vintage.Helper;

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
        /// 提供与Uri相关的帮助方法
        /// </summary>
        private readonly IUrlHelper _urlHelper;
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment _environment;
        private readonly MarkdownHelper _markdownHelper;

        /// <summary>
        /// 构造<see cref="PostsController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public PostsController(IWebHostEnvironment env, ILogger<HomeController> logger, IUrlHelper urlHelper)
        {
            _environment = env;
            _logger = logger;
            _urlHelper = urlHelper;
            _markdownHelper = new();
        }

        /// <summary>
        /// 查看期刊列表
        /// </summary>
        public IActionResult Index()
        {
            IndexViewModel model = new();
            string path = Path.Combine(_environment.WebRootPath, "aneot", "posts", "README.md");
            string markdown = SystemIOFile.ReadAllText(path);
            PostInfo postInfo = _markdownHelper.GetFrontMatter<PostInfo>(markdown);
            model.Title = postInfo.Title;
            model.PostInfo = postInfo;
            model.Markdown = _markdownHelper.ReplaceUriAsAbsolute(markdown, string.Empty, _urlHelper);
            return base.View(model);
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

            if (article is not null)
            {
                if (article.Contains(".html"))
                {
                    //后缀为.html，这是原网站的写法，要重定向
                    return LocalRedirectPermanent($"~/posts/{post}/{article.Replace(".html", ".md")}");
                }
                else if (!article.Contains(".md"))
                {
                    //Uri后缀不带.md，这是找不到文章的，也要重定向
                    return LocalRedirectPermanent($"~/posts/{post}/{article}.md");
                }
            }

            string path;
            string markdown;
            ViewViewModel model = new();

            if (string.IsNullOrWhiteSpace(article))
            {
                //操作：查看指定的期刊
                path = Path.Combine(_environment.WebRootPath, "aneot", "posts", post, "README.md");

                if (!SystemIOFile.Exists(path))
                {
                    return NotFound();
                }

                markdown = SystemIOFile.ReadAllText(path);
            }
            else
            {
                //操作：查看指定的文章
                path = Path.Combine(_environment.WebRootPath, "aneot", "posts", post, article);

                if (!SystemIOFile.Exists(path))
                {
                    return NotFound();
                }

                markdown = SystemIOFile.ReadAllText(path);
            }
            PostInfo postInfo = _markdownHelper.GetFrontMatter<PostInfo>(markdown);
            model.Title = postInfo.Title;
            model.PostInfo = postInfo;
            model.Markdown = _markdownHelper.ReplaceUriAsAbsolute(markdown, post, _urlHelper);
            model.CurrentPost = post;

            return base.View(model);
        }
    }
}
