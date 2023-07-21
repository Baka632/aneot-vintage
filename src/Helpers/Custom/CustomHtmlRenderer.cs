using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace AnEoT.Vintage.Helpers.Custom
{
    /// <summary>
    /// 自定义的HTML渲染器
    /// </summary>
    public class CustomHtmlRenderer : HtmlRenderer
    {
        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomHtmlRenderer"/>的新实例
        /// </summary>
        public CustomHtmlRenderer(TextWriter writer, bool convertWebP, string? baseUri = null) : base(writer)
        {
            IMarkdownObjectRenderer target = ObjectRenderers.First(obj => obj is LinkInlineRenderer);
            
            int targetIndex = ObjectRenderers.IndexOf(target);
            ObjectRenderers.Insert(targetIndex, new CustomLinkInlineRenderer(convertWebP, baseUri));
            ObjectRenderers.Remove(target);
        }
    }
}
