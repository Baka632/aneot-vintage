using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 信息页控制器
    /// </summary>
    [Route("download")]
    [Route("call")]
    [Route("subscription")]
    [Route("forceflash")]
    public class InfomationPageController : Controller
    {
        /// <summary>
        /// 查看请求的信息页
        /// </summary>
        public IActionResult Index([FromRoute]string name)
        {
            return View("test");
        }
    }
}
