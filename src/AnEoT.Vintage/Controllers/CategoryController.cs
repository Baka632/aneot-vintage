using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 分类页面控制器
    /// </summary>
    /// <param name="env">
    /// 程序执行环境的信息提供者
    /// </param>
    public class CategoryController(IWebHostEnvironment env) : Controller
    {
        /// <summary>
        /// 显示分类页面
        /// </summary>
        public IActionResult Index()
        {
            string webRootPath = env.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryAndTagHelper.GetCategoryToArticleMapping(webRootPath);

            IndexViewModel model = new(mapping);
            return View(model);
        }

        /// <summary>
        /// 显示分类详细信息页
        /// </summary>
        [Route("Category/{category}")]
        public IActionResult Detail(string category)
        {
            string webRootPath = env.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryAndTagHelper.GetCategoryToArticleMapping(webRootPath);

            if (mapping.ContainsKey(category) != true)
            {
                return NotFound();
            }

            DetailViewModel model = new(category, mapping[category]);
            return View(model);
        }
    }
}
