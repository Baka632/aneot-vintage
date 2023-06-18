using AnEoT.Vintage.Helper;
using AnEoT.Vintage.Models;
using AnEoT.Vintage.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 信息页控制器
    /// </summary>
    public class InfomationPageController : Controller
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
        /// 构造<see cref="InfomationPageController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public InfomationPageController(IWebHostEnvironment env, ILogger<HomeController> logger, IUrlHelper urlHelper)
        {
            _environment = env;
            _logger = logger;
            _urlHelper = urlHelper;
            _markdownHelper = new();
        }

        private ViewViewModel GetViewViewModel(string markdownFilePath, string? baseUriOverride = null)
        {
            ViewViewModel model = new();
            string markdown = SystemIOFile.ReadAllText(markdownFilePath);
            PostInfo postInfo = _markdownHelper.GetFrontMatter<PostInfo>(markdown);
            model.Title = postInfo.Title;
            model.PostInfo = postInfo;
            model.Markdown = _markdownHelper.ReplaceUriAsAbsolute(markdown, string.Empty, _urlHelper, baseUriOverride);
            return model;
        }

        /// <summary>
        /// 查看下载页
        /// </summary>
        [Route("download")]
        [Route("download.md")]
        public IActionResult DownloadPage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "download.md");
            ViewViewModel model = GetViewViewModel(path);
            return View("Views/Posts/View.cshtml", model);
        }

        /// <summary>
        /// 查看征稿启事页
        /// </summary>
        [Route("call")]
        [Route("call.md")]
        public IActionResult CallPage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "call.md");
            ViewViewModel model = GetViewViewModel(path, string.Empty);
            return View("Views/Posts/View.cshtml", model);
        }

        /// <summary>
        /// 查看订阅页
        /// </summary>
        [Route("subscription")]
        [Route("subscription.md")]
        public IActionResult SubscriptionPage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "subscription.md");
            ViewViewModel model = GetViewViewModel(path, "https://aneot.terrach.net");
            return View("Views/Posts/View.cshtml", model);
        }

        /// <summary>
        /// 查看投稿指南页
        /// </summary>
        [Route("guidance")]
        [Route("guidance.md")]
        public IActionResult GuidancePage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "guidance.md");
            ViewViewModel model = GetViewViewModel(path);
            return View("Views/Posts/View.cshtml", model);
        }
        
        /// <summary>
        /// 查看关于页
        /// </summary>
        [Route("intro")]
        [Route("intro.md")]
        public IActionResult IntroPage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "intro.md");
            ViewViewModel model = GetViewViewModel(path);
            return View("Views/Posts/View.cshtml", model);
        }
        
        /// <summary>
        /// 查看用户协议页
        /// </summary>
        [Route("statement")]
        [Route("statement.md")]
        public IActionResult StatementPage()
        {
            string path = Path.Combine(_environment.WebRootPath, "aneot", "statement.md");
            ViewViewModel model = GetViewViewModel(path, "~");
            return View("Views/Posts/View.cshtml", model);
        }

        //TODO: 待完成板块显示页后再去除注释

        ///// <summary>
        ///// 查看板块说明页
        ///// </summary>
        //[Route("description")]
        //[Route("description.md")]
        //public IActionResult DescriptionPage()
        //{
        //    string path = Path.Combine(_environment.WebRootPath, "aneot", "description.md");
        //    ViewViewModel model = GetViewViewModel(path);
        //    return View("Views/Posts/View.cshtml", model);
        //}

        /// <summary>
        /// 不需要
        /// </summary>
        [Route("forceflash")]
        public IActionResult ForceFlashPage()
        {
            return Ok();
        }
    }
}
