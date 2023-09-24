using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的<see cref="HtmlBlock"/>渲染器
/// </summary>
public class CustomHtmlBlockRenderer : HtmlBlockRenderer
{
    private readonly bool convertWebP;
    private readonly bool noAd;

    /// <summary>
    /// 使用指定的参数构造<seealso cref="CustomHtmlBlockRenderer"/>的新实例
    /// </summary>
    public CustomHtmlBlockRenderer(bool convertWebP, bool noAd)
    {
        this.convertWebP = convertWebP;
        this.noAd = noAd;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlBlock obj)
    {
        StringLine[] slices = obj.Lines.Lines;

        if (slices is not null)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                ref StringSlice slice = ref slices[i].Slice;
                if (slice.Text is null)
                {
                    break;
                }

                string html = slice.ToString();
                HtmlParser parser = new();
                using IHtmlDocument document = parser.ParseDocument(html);

                IElement? fakeAd = document.All
                        .FirstOrDefault(element => element.TagName.ToUpperInvariant() is "FAKEADS");

                if (convertWebP)
                {
                    IElement? element = document.All
                        .Where(element => element.TagName.ToUpperInvariant() is "IMG").FirstOrDefault();

                    if (element is IHtmlImageElement image)
                    {
                        string? originalSrc = image.GetAttribute("src");
                        if (originalSrc is not null)
                        {
                            image.SetAttribute("src", originalSrc.Replace(".webp", ".jpg"));

                            using StringWriter writer = new();
                            PrettyMarkupFormatter formatter = new();
                            image.ToHtml(writer, formatter);

                            string text = writer.ToString().Trim();
                            slice = new StringSlice(text);
                        }
                    }
                }

                if (fakeAd is not null && noAd is not true)
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

                    slice = new StringSlice(fakeAdHtml);
                }
            }
        }

        base.Write(renderer, obj);
    }
}
