﻿using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage.Helpers.Custom
{
    /// <summary>
    /// 自定义的Markdown解析器工厂
    /// </summary>
    public class CustomMarkdownParserFactory : IMarkdownParserFactory
    {
        private bool convertWebP;

        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomMarkdownParserFactory"/>的新实例
        /// </summary>
        public CustomMarkdownParserFactory(bool convertWebP)
        {
            this.convertWebP = convertWebP;
        }

        /// <inheritdoc/>
        public IMarkdownParser? CurrentParser { get; private set; }

        /// <inheritdoc/>
        public IMarkdownParser GetParser(bool usePragmaLines = false, bool forceLoad = false)
        {
            if (!forceLoad && CurrentParser != null)
            {
                return CurrentParser;
            }

            CurrentParser = new CustomMarkdownParser(usePragmaLines, forceLoad, convertWebP);
            return CurrentParser;
        }
    }
}