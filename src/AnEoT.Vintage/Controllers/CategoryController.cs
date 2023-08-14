using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 分类页面控制器
    /// </summary>
    public class CategoryController : Controller
    {
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// 构造<see cref="CategoryController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public CategoryController(IWebHostEnvironment env)
        {
            environment = env;
        }

        /// <summary>
        /// 显示分类页面
        /// </summary>
        public IActionResult Index()
        {
            string webRootPath = environment.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryHelper.GetCategoryToArticleMapping(webRootPath);

            IndexViewModel model = new(mapping);
            return View(model);
        }

        /// <summary>
        /// 显示分类详细信息页
        /// </summary>
        [Route("Category/{category}")]
        public IActionResult Detail(string category)
        {
            string webRootPath = environment.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryHelper.GetCategoryToArticleMapping(webRootPath);

            if (mapping.ContainsKey(category) != true)
            {
                return NotFound();
            }

            DetailViewModel model = new(category, mapping[category]);
            return View(model);
        }
    }
}
