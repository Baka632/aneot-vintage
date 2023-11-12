using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 设置页面控制器
    /// </summary>
    public class SettingsController : Controller
    {
        /// <summary>
        /// 显示设置页面
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
    }
}
