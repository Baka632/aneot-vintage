using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers;

/// <summary>
/// 安装 PWA 应用页面控制器。
/// </summary>
public class InstallPwaController : Controller
{
    /// <summary>
    /// 显示 PWA 应用安装页面。
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }
}
