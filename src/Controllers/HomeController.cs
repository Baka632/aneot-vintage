using AnEoT.Vintage.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;
using AnEoT.Vintage.Models.HomePage;
using AnEoT.Vintage.Helper;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 主页面控制器
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<HomeController> _logger;
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;
        private readonly MarkdownHelper _markdownHelper;

        /// <summary>
        /// 构造<see cref="HomeController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            environment = env;
            _markdownHelper = new();
        }

        /// <summary>
        /// 显示主页面
        /// </summary>
        public IActionResult Index()
        {
            IndexViewModel model = new();

            string path = Path.Combine(environment.WebRootPath, "aneot", "README.md");
            string markdown = SystemIOFile.ReadAllText(path);

            model.HomePageInfo = _markdownHelper.GetFrontMatter<HomePageInfo>(markdown);
            return View(model);
        }
    }
}