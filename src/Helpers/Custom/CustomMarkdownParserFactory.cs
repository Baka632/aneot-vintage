using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage.Helpers.Custom
{
    /// <summary>
    /// 自定义的Markdown解析器工厂
    /// </summary>
    public class CustomMarkdownParserFactory : IMarkdownParserFactory
    {
        /// <inheritdoc/>
        public IMarkdownParser? CurrentParser { get; private set; }

        /// <inheritdoc/>
        public IMarkdownParser GetParser(bool usePragmaLines = false, bool forceLoad = false)
        {
            if (!forceLoad && CurrentParser != null)
            {
                return CurrentParser;
            }

            CurrentParser = new CustomMarkdownParser(usePragmaLines, forceLoad);
            return CurrentParser;
        }
    }
}
