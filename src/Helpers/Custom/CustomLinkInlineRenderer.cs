using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AnEoT.Vintage.Helpers.Custom
{
    /// <summary>
    /// 自定义的链接渲染器
    /// </summary>
    public class CustomLinkInlineRenderer : LinkInlineRenderer
    {
        private readonly bool convertWebP;

        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomLinkInlineRenderer"/>的新实例
        /// </summary>
        public CustomLinkInlineRenderer(bool convertWebP)
        {
            this.convertWebP = convertWebP;
        }

        /// <inheritdoc/>
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (link.Url is not null && new Uri(link.Url, UriKind.RelativeOrAbsolute).IsAbsoluteUri is not true)
            {
                link.Url = link.Url?.Replace(".md", ".html").Replace("README", "index");
            }

            if (convertWebP && link.IsImage)
            {
                link.Url = link.Url?.Replace(".webp", ".jpg");
            }

            base.Write(renderer, link);
        }
    }
}
