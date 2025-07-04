using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;
using AnEoT.Vintage.Models.VueComponentAbstractions;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的 <see cref="HtmlInline"/> 渲染器。
/// </summary>
public class CustomHtmlInlineRenderer : HtmlInlineRenderer
{
    private readonly bool noEod;

    /// <summary>
    /// 使用指定的参数构造 <seealso cref="CustomHtmlInlineRenderer"/> 的新实例。
    /// </summary>
    public CustomHtmlInlineRenderer(bool noEod)
    {
        this.noEod = noEod;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlInline obj)
    {
        if (noEod is not true)
        {
            HtmlParser parser = new();
            using IHtmlDocument document = parser.ParseDocument(obj.Tag);
            IElement? element = document.Body?.FirstElementChild;

            if (element != null)
            {
                if (element.TagName == Eod.TagName)
                {
                    obj.Tag = Eod.GetHtml();
                }
            }
        }

        base.Write(renderer, obj);
    }
}
