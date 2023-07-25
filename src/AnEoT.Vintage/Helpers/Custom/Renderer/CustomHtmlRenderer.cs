using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;

namespace AnEoT.Vintage.Helpers.Custom.Renderer
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
            {
                IMarkdownObjectRenderer linkInlineRenderer = ObjectRenderers.First(obj => obj is LinkInlineRenderer);

                int linkInlineRendererIndex = ObjectRenderers.IndexOf(linkInlineRenderer);
                ObjectRenderers.Insert(linkInlineRendererIndex, new CustomLinkInlineRenderer(convertWebP, baseUri));
                ObjectRenderers.Remove(linkInlineRenderer);
            }

            {
                IMarkdownObjectRenderer htmlBlockRenderer = ObjectRenderers.First(obj => obj is HtmlBlockRenderer);
                int htmlBlockRendererIndex = ObjectRenderers.IndexOf(htmlBlockRenderer);
                ObjectRenderers.Insert(htmlBlockRendererIndex, new CustomHtmlBlockRenderer(convertWebP));
                ObjectRenderers.Remove(htmlBlockRenderer);
            }
        }
    }
}
