using AnEoT.Vintage.Models;
using AnEoT.Vintage.Models.VueComponentAbstractions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;
using VolumeInfo = AnEoT.Vintage.Models.VueComponentAbstractions.VolumeInfo;

namespace AnEoT.Vintage.Helpers.Custom.Renderer;

/// <summary>
/// 自定义的 <see cref="HtmlInline"/> 渲染器。
/// </summary>
public class CustomHtmlInlineRenderer : HtmlInlineRenderer
{
    private readonly bool noEod;
    private readonly string webRootPath;
    private readonly bool convertWebp;

    /// <summary>
    /// 使用指定的参数构造 <seealso cref="CustomHtmlInlineRenderer"/> 的新实例。
    /// </summary>
    public CustomHtmlInlineRenderer(bool noEod, string webRootPath, bool convertWebp)
    {
        this.noEod = noEod;
        this.webRootPath = webRootPath;
        this.convertWebp = convertWebp;
    }

    /// <inheritdoc/>
    protected override void Write(HtmlRenderer renderer, HtmlInline obj)
    {
        HtmlParser parser = new();
        using IHtmlDocument document = parser.ParseDocument(obj.Tag);
        IElement? currentElement = document.Body?.FirstElementChild;

        if (currentElement != null)
        {
            string tagName = currentElement.TagName;

            if (noEod is not true && tagName == Eod.TagName)
            {
                obj.Tag = Eod.GetHtml();
            }
            else if (tagName == VolumeInfo.TagName)
            {
                string? typeString = currentElement.GetAttribute("type") ?? throw new InvalidOperationException("HTML 元素 VolumeInfo 的属性“type”是必须的。");
                VolumeInfoType type = typeString switch
                {
                    "latest-title" => VolumeInfoType.LatestTitle,
                    "latest-cover" => VolumeInfoType.LatestCover,
                    "latest-link" => VolumeInfoType.LatestLink,
                    "volumes" => VolumeInfoType.Volumes,
                    _ => throw new InvalidOperationException("无法识别 HTML 元素 VolumeInfo 的类型。")
                };

                obj.Tag = VolumeInfo.GetHtml(type, webRootPath, convertWebp);
            }

            if (convertWebp)
            {
                IElement? element = document.All
                    .Where(element => element.TagName.ToUpperInvariant() is "IMG" or "STYLE").FirstOrDefault();

                if (element is IHtmlImageElement image)
                {
                    string? originalSrc = image.GetAttribute("src");
                    if (originalSrc is not null)
                    {
                        image.SetAttribute("src", originalSrc.Replace(".webp", ".jpg"));
                        obj.Tag = document.Body?.InnerHtml ?? string.Empty;
                    }
                }
            }
        }

        base.Write(renderer, obj);
    }
}
