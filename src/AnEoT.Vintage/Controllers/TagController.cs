using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 标签页面控制器
    /// </summary>
    public class TagController : Controller
    {
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// 构造<see cref="TagController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public TagController(IWebHostEnvironment env)
        {
            environment = env;
        }

        /// <summary>
        /// 显示标签页面
        /// </summary>
        public IActionResult Index()
        {
            string webRootPath = environment.WebRootPath;
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
            string webRootPath = environment.WebRootPath;
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
