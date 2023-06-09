using AnEoT.Vintage.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// 构造<see cref="HomeController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            environment = env;
        }

        /// <summary>
        /// 显示最新文章
        /// </summary>
        public IActionResult Index()
        {
            //TODO: 迁移到显示最新文章

            string path = Path.Combine(environment.WebRootPath, "aneot", "posts");

            if (!Directory.Exists(path))
            {
                _logger.LogCritical("未能找到含有《回归线》内容的文件夹！使用的路径：{path}", path);
                return NotFound();
            }

            DirectoryInfo directoryInfo = new(path);
            IEnumerable<DirectoryInfo> directories = directoryInfo.EnumerateDirectories();

            return View(new IndexViewModel(directories));
        }
    }
}