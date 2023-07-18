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
        private readonly string? baseUri;

        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomLinkInlineRenderer"/>的新实例
        /// </summary>
        public CustomLinkInlineRenderer(bool convertWebP, string? baseUri = null)
        {
            this.convertWebP = convertWebP;
            this.baseUri = baseUri;
        }

        /// <inheritdoc/>
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (link.Url is not null)
            {
                bool isAbsoluteUri = new Uri(link.Url, UriKind.RelativeOrAbsolute).IsAbsoluteUri;

                if (baseUri is not null && !isAbsoluteUri)
                {
                    if (baseUri.EndsWith('/'))
                    {
                        renderer.BaseUrl = new Uri(baseUri, UriKind.Absolute);
                    }
                    else
                    {
                        renderer.BaseUrl = new Uri($"{baseUri}/", UriKind.Absolute);
                    }
                }

                if (isAbsoluteUri is not true)
                {
                    link.Url = link.Url?.Replace(".md", ".html").Replace("README", "index");
                }

                if (convertWebP && link.IsImage)
                {
                    link.Url = link.Url?.Replace(".webp", ".jpg");
                }
            }

            base.Write(renderer, link);
        }
    }
}
