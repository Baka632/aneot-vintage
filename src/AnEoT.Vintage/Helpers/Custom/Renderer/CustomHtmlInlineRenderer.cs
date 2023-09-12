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
    private readonly bool convertWebP;

    /// <summary>
    /// 使用指定的参数构造<seealso cref="CustomHtmlInlineRenderer"/>的新实例
    /// </summary>
    public CustomHtmlInlineRenderer(bool convertWebP)
    {
        this.convertWebP = convertWebP;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlInline obj)
    {
        if (obj.Tag.Equals("<eod />", StringComparison.OrdinalIgnoreCase))
        {
            obj.Tag = """
                <span><img src="/eod.jpg" /></span>
                """;
        }
        else if (obj.Tag.Equals("<FakeAds />", StringComparison.OrdinalIgnoreCase))
        {
            Models.FakeAdInfo ad = FakeAdHelper.RollFakeAd(convertWebP);
            string fakeAdHtml = $"""
                <div class="ads-container no-print">
                    <p class="ads-hint">{ad.AdText}<a href="{ad.AboutLink}">{ad.AdAbout}</a></p>
                    <div class="image-container">
                      <a href="{ad.AdLink}" target="/" rel="noopener noreferrer">
                        <img src="/fake-ads/{ad.AdImageLink}" alt="Advertisement" />
                      </a>
                    </div>
                </div>
                """;

            obj.Tag = fakeAdHtml;
        }

        base.Write(renderer, obj);
    }
}
