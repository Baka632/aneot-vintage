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

                if (convertWebP)
                {
                    string html = slice.ToString();

                    HtmlParser parser = new();
                    using IHtmlDocument document = parser.ParseDocument(html);
                    IEnumerable<IElement> imgElements = document.All
                        .Where(element => element.TagName.ToUpperInvariant() is "IMG");

                    bool isModified = false;

                    foreach (IElement element in imgElements)
                    {
                        if (element is IHtmlImageElement image)
                        {
                            string? originalSrc = image.GetAttribute("src");
                            if (originalSrc is not null)
                            {
                                image.SetAttribute("src", originalSrc.Replace(".webp", ".jpg"));
                                isModified = true;
                            }
                        }
                    }

                    if (isModified)
                    {
                        using StringWriter writer = new();

                        PrettyMarkupFormatter formatter = new();
                        document.ToHtml(writer, formatter);

                        slice = new StringSlice(writer.ToString());
                    }
                }
            }
        }

        base.Write(renderer, obj);
    }
}
