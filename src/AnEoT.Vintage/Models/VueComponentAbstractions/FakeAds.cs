using System.Globalization;
using System.Text;

namespace AnEoT.Vintage.Models.VueComponentAbstractions;

/// <summary>
/// 「泰拉广告」元素
/// </summary>
public static class FakeAds
{
    /// <summary>
    /// 元素标签名。
    /// </summary>
    public const string TagName = "FAKEADS";

    /// <summary>
    /// 元素 HTML 模板
    /// </summary>
    public const string Template = """
                <div class="ads-container no-print{0}">
                    <p class="ads-hint">{1}<a href="{2}">{3}</a></p>
                    <div class="image-container">
                      <a href="{4}" target="/" rel="noopener noreferrer">
                        <img src="/fake-ads/{5}" alt="Advertisement" />
                      </a>
                    </div>
                </div>
                """;

    private readonly static CompositeFormat TemplateFormat = CompositeFormat.Parse(Template);

    /// <summary>
    /// 获取 <see cref="FakeAds"/> 的 HTML。
    /// </summary>
    /// <param name="fakeAdInfo">表示「泰拉广告」信息的 <see cref="FakeAdInfo"/>。</param>
    /// <param name="optionalClassName">可选的类名，以空格分割。</param>
    /// <returns>构造好的 <see cref="FakeAds"/> 的 HTML</returns>
    public static string GetHtml(FakeAdInfo fakeAdInfo, string optionalClassName = "")
    {
        return string.Format(CultureInfo.InvariantCulture, TemplateFormat,
                             $" {optionalClassName}",
                             fakeAdInfo.AdText,
                             fakeAdInfo.AboutLink,
                             fakeAdInfo.AdAbout,
                             fakeAdInfo.AdLink,
                             fakeAdInfo.AdImageLink);
    }
}
