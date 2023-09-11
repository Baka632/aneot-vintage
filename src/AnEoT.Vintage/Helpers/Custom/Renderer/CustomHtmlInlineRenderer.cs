using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的 <see cref="HtmlInline"/> 渲染器
/// </summary>
public class CustomHtmlInlineRenderer : HtmlInlineRenderer
{
    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlInline obj)
    {
        if (obj.Tag.Equals("<eod />", StringComparison.OrdinalIgnoreCase))
        {
            obj.Tag = """
                <span><img src="/eod.jpg" /></span>
                """;
        }

        base.Write(renderer, obj);
    }
}
