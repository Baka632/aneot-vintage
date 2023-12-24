using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 标签页面控制器
    /// </summary>
    /// <param name="env">
    /// 程序执行环境的信息提供者
    /// </param>
    public class TagController(IWebHostEnvironment env) : Controller
    {

        /// <summary>
        /// 显示标签页面
        /// </summary>
        public IActionResult Index()
        {
            string webRootPath = env.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryAndTagHelper.GetTagToArticleMapping(webRootPath);

            IndexViewModel model = new(mapping);
            return View(model);
        }

        /// <summary>
        /// 显示分类详细信息页
        /// </summary>
        [Route("Tag/{tag}")]
        public IActionResult Detail(string tag)
        {
            string webRootPath = env.WebRootPath;
            IDictionary<string, List<string>> mapping = CategoryAndTagHelper.GetTagToArticleMapping(webRootPath);

            if (mapping.ContainsKey(tag) != true)
            {
                return NotFound();
            }

            DetailViewModel model = new(tag, mapping[tag]);
            return View(model);
        }
    }
}
