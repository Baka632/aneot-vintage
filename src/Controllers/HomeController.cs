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
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// 构造<see cref="HomeController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public HomeController(IWebHostEnvironment env)
        {
            environment = env;
        }

        /// <summary>
        /// 显示主页面
        /// </summary>
        public IActionResult Index()
        {
            IndexViewModel model = new();

            //不用原来的"README.md"为文件名的原因是：README.md是默认文档，Markdown中间件会拦截它
            //这里的控制器就无法被路由到
            string path = Path.Combine(environment.WebRootPath, "Homepage.md");
            string markdown = SystemIOFile.ReadAllText(path);

            model.HomePageInfo = MarkdownHelper.GetFrontMatter<HomePageInfo>(markdown);
            return View(model);
        }
    }
}