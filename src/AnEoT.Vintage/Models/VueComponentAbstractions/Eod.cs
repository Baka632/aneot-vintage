using System.Globalization;
using System.Text;

namespace AnEoT.Vintage.Models.VueComponentAbstractions;

/// <summary>
/// 文章结束元素（&lt;eod /&gt;）。
/// </summary>
public static class Eod
{
    /// <summary>
    /// 元素标签名。
    /// </summary>
    public const string TagName = "EOD";

    /// <summary>
    /// 元素 HTML 模板
    /// </summary>
    private const string Template = """<span class="no-interact{0}"><img id="eod-image-element" src="/eod.jpg" /></span>""";

    private readonly static CompositeFormat TemplateFormat = CompositeFormat.Parse(Template);

    /// <summary>
    /// 获取 <see cref="Eod"/> 的 HTML。
    /// </summary>
    /// <param name="optionalClassName">可选的类名，以空格分割。</param>
    /// <returns>构造好的 <see cref="Eod"/> 的 HTML</returns>
    public static string GetHtml(string optionalClassName = "")
    {
        return string.Format(CultureInfo.InvariantCulture, TemplateFormat, $" {optionalClassName}");
    }
}