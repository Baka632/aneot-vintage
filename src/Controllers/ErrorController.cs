using AnEoT.Vintage.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 错误处理控制器
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public partial class ErrorController : Controller
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// 构造<see cref="ErrorController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public ErrorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 显示HTTP 错误状态码处理页
        /// </summary>
        /// <param name="code">HTTP 状态码</param>
        [Route("[controller]/{code}")]
        public IActionResult ErrorStatusCodePages(int code)
        {
            ErrorViewModel model = new();

            string mainMessage;
            string secondaryMessage;

            switch (code)
            {
                case StatusCodes.Status404NotFound:
                    mainMessage = "页面不存在";
                    secondaryMessage = "这 是 四 零 四 !";
                    break;
                case StatusCodes.Status403Forbidden:
                    mainMessage = "无权查看";
                    secondaryMessage = "打 咩 打 咩 !";
                    break;
                case StatusCodes.Status400BadRequest:
                    mainMessage = "请求无效";
                    secondaryMessage = "你的链接输对了吗？";
                    break;
                default:
                    mainMessage = "出现了未知错误 :(";
                    secondaryMessage = "你有没有听见服务器的悲鸣？";
                    break;
            }

            model.MainMessage = mainMessage;
            model.SecondaryMessage = secondaryMessage;
            return View("Error", model);
        }

        /// <summary>
        /// 非开发环境的异常处理器
        /// </summary>
        public IActionResult HandleError()
        {
            IExceptionHandlerPathFeature? expectionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            LogRequestError(expectionDetails?.Path);

            ErrorViewModel model = new()
            {
                MainMessage = "出现了未知错误 :(",
                SecondaryMessage = "你有没有听见服务器的悲鸣？"
            };
            return View("Error", model);
        }

        [LoggerMessage(Level = LogLevel.Error, Message = "出现错误。请求的 Uri 为：{requestUri}")]
        internal partial void LogRequestError(string? requestUri);
    }
}
