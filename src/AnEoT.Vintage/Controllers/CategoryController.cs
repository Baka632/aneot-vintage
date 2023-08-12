using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 分类页面控制器
    /// </summary>
    public class CategoryController : Controller
    {
        /// <summary>
        /// 显示分类页面
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
    }
}
