using Markdig.Renderers;
using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage.Helpers.Custom
{
    /// <summary>
    /// 自定义Markdown解析器
    /// </summary>
    public class CustomMarkdownParser : MarkdownParserMarkdig
    {
        private readonly bool convertWebP;
        private readonly string? baseUri;

        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomMarkdownParser"/>的新实例
        /// </summary>
        public CustomMarkdownParser(bool usePragmaLines, bool forceLoad, bool convertWebP, string? baseUri = null) : base(usePragmaLines, forceLoad)
        {
            this.convertWebP = convertWebP;
            this.baseUri = baseUri;
        }

        /// <inheritdoc/>
        public override string Parse(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            string html;
            using (StringWriter stringWriter = new())
            {
                IMarkdownRenderer renderer = new CustomHtmlRenderer(stringWriter, convertWebP, baseUri);
                Markdig.Markdown.Convert(markdown, renderer, Pipeline);
                html = stringWriter.ToString();
            }

            html = ParseFontAwesomeIcons(html);
            if (sanitizeHtml)
            {
                html = Sanitize(html);
            }

            return html;
        }
    }
}
