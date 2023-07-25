using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace AnEoT.Vintage.Helpers.Custom;

/// <summary>
/// 自定义的<see cref="HtmlBlock"/>渲染器
/// </summary>
public class CustomHtmlBlockRenderer : HtmlBlockRenderer
{
    private readonly bool convertWebP;

    /// <summary>
    /// 使用指定的参数构造<seealso cref="CustomHtmlBlockRenderer"/>的新实例
    /// </summary>
    public CustomHtmlBlockRenderer(bool convertWebP)
    {
        this.convertWebP = convertWebP;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlBlock obj)
    {
        base.Write(renderer, obj);
    }
}
