using System;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的 <see cref="HtmlInline"/> 渲染器
/// </summary>
public class CustomHtmlInlineRenderer : HtmlInlineRenderer
{
    private readonly bool noEod;

    /// <summary>
    /// 使用指定的参数构造<seealso cref="CustomHtmlInlineRenderer"/>的新实例
    /// </summary>
    public CustomHtmlInlineRenderer(bool noEod)
    {
        this.noEod = noEod;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlInline obj)
    {
        if (obj.Tag.Equals("<eod />", StringComparison.OrdinalIgnoreCase) && noEod is not true)
        {
            obj.Tag = """
                <span>
                    <picture>
                        <source srcset="/eod_white.svg" width="14" height="14" type="image/svg+xml" media="(prefers-color-scheme: dark)" />
                        <source srcset="/eod_black.svg" width="14" height="14" type="image/svg+xml" />
                        <img src="/eod.jpg" srcset="/eod_auto.svg" />
                    </picture>
                </span>
                """;
        }

        base.Write(renderer, obj);
    }
}
