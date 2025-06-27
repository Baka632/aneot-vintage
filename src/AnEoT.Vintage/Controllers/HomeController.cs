using AnEoT.Vintage.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 主页面控制器
    /// </summary>
    /// <param name="env">
    /// 程序执行环境的信息提供者
    /// </param>
    public class HomeController(IWebHostEnvironment env) : Controller
    {

        /// <summary>
        /// 显示主页面
        /// </summary>
        public IActionResult Index()
        {
            string path = Path.Combine(env.WebRootPath, "README.md");
            string markdown = SystemIOFile.ReadAllText(path);
            HomePageInfo info = MarkdownHelper.GetFromFrontMatter<HomePageInfo>(markdown);
            IndexViewModel model = new(info);
            return View(model);
        }
    }
}