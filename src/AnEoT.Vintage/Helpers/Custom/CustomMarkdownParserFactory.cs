using Westwind.AspNetCore.Markdown;

namespace AnEoT.Vintage.Helpers.Custom;

/// <summary>
/// 自定义的 Markdown 解析器工厂。
/// </summary>
public class CustomMarkdownParserFactory : IMarkdownParserFactory
{
    private readonly bool convertWebP;
    private readonly string? baseUri;
    private readonly string webRootPath;

    /// <summary>
    /// 使用指定的参数构造 <seealso cref="CustomMarkdownParserFactory"/> 的新实例。
    /// </summary>
    public CustomMarkdownParserFactory(bool convertWebP, string webRootPath, string? baseUri = null)
    {
        this.convertWebP = convertWebP;
        this.baseUri = baseUri;
        this.webRootPath = webRootPath;
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

        CurrentParser = new CustomMarkdownParser(usePragmaLines, forceLoad, convertWebP, webRootPath, baseUri);
        return CurrentParser;
    }
}
