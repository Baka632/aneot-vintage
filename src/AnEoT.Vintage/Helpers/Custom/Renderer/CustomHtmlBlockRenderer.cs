using System.Text;
using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Values;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using AnEoT.Vintage.Models.VueComponentAbstractions;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的 <see cref="HtmlBlock"/> 渲染器。
/// </summary>
public class CustomHtmlBlockRenderer : HtmlBlockRenderer
{
    private readonly bool convertWebP;
    private readonly bool noAd;
    private readonly bool noEod;

    /// <summary>
    /// 使用指定的参数构造 <seealso cref="CustomHtmlBlockRenderer"/> 的新实例。
    /// </summary>
    public CustomHtmlBlockRenderer(bool convertWebP, bool noAd, bool noEod)
    {
        this.convertWebP = convertWebP;
        this.noAd = noAd;
        this.noEod = noEod;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlBlock obj)
    {
        StringLine[] slices = obj.Lines.Lines;

        StringBuilder builder = new(slices.Length);
        foreach (var item in slices)
        {
             builder.AppendLine(item.Slice.ToString());
        }
        string elementHtml = builder.ToString();

        AngleSharp.IConfiguration config = Configuration.Default.WithCss();
        IBrowsingContext context = BrowsingContext.New(config);

        HtmlParser parser = new(default, context);
        using IHtmlDocument document = parser.ParseDocument(elementHtml);

        IElement? currentElement = document.Body?.FirstElementChild;

        if (currentElement is not null)
        {
            string classNames = currentElement.ClassList is not null && currentElement.ClassList.Length != 0
                ? string.Join(' ', currentElement.ClassList)
                : string.Empty;

            if (!noAd && currentElement.TagName == FakeAds.TagName)
            {
                Models.FakeAdInfo ad = FakeAdHelper.RollFakeAd(convertWebP);
                string fakeAdHtml = FakeAds.GetHtml(ad, classNames);

                obj.Lines = new StringLineGroup(fakeAdHtml);
            }
            else if (!noEod && currentElement.TagName == Eod.TagName)
            {
                string eodHtml = Eod.GetHtml(classNames);

                obj.Lines = new StringLineGroup(eodHtml);
            }
        }

        if (convertWebP)
        {
            IElement? element = document.All
                .Where(element => element.TagName.ToUpperInvariant() is "IMG" or "STYLE").FirstOrDefault();

            if (element is IHtmlImageElement image)
            {
                string? originalSrc = image.GetAttribute("src");
                if (originalSrc is not null)
                {
                    image.SetAttribute("src", originalSrc.Replace(".webp", ".jpg"));
                    string text = document.Body?.InnerHtml ?? string.Empty;
                    obj.Lines = new StringLineGroup(text);
                }
            }
            else if (element is IHtmlStyleElement styleElement)
            {
                ICssStyleSheet? sheet = (ICssStyleSheet?)styleElement.Sheet;
                if (sheet != null)
                {
                    foreach (ICssStyleRule rule in sheet.Rules.Cast<ICssStyleRule>())
                    {
                        ICssValue cssValue = rule.GetValueOf("background-image");
                        if (cssValue is CssListValue<ICssValue> cssListValue)
                        {
                            for (int i = 0; i < cssListValue.Items.Length; i++)
                            {
                                ICssValue item = cssListValue.Items[i];
                                if (item is CssUrlValue urlValue && urlValue.Path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                                {
                                    cssListValue.Items[i] = new CssUrlValue(urlValue.Path.Replace(".webp", ".jpg"));
                                }
                            }
                        }
                    }

                    using StringWriter cssWriter = new();
                    sheet.ToCss(cssWriter, new CssStyleFormatter());
                    styleElement.InnerHtml = cssWriter.ToString();

                    using StringWriter writer = new();
                    PrettyMarkupFormatter formatter = new();
                    styleElement.ToHtml(writer, formatter);

                    string text = writer.ToString().Trim();
                    obj.Lines = new StringLineGroup(text);
                }
            }
        }

        base.Write(renderer, obj);
    }
}
