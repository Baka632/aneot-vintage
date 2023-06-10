using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.ViewModels.Posts
{
    /// <summary>
    /// 为Posts/Index页面提供信息的模型
    /// </summary>
    public class IndexViewModel
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
