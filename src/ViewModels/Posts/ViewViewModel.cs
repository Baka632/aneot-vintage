using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.ViewModels.Posts
{
    //这类的名字有点抽象（

    /// <summary>
    /// 为Posts/View页面提供信息的模型
    /// </summary>
    public class ViewViewModel
    {
        /// <summary>
        /// 本页面的标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 本页面的Markdown内容
        /// </summary>
        public string? Markdown { get; set; }

        /// <summary>
        /// 本页面的Markdown内容信息
        /// </summary>
        public PostInfo PostInfo { get; set; }
    }
}
