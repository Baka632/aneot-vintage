using System.Text;
using System.Linq;

namespace AnEoT.Vintage.Common.Helpers;

/// <summary>
/// 为文字计数提供帮助的类。
/// </summary>
public static class WordCountHelper
{
    /// <summary>
    /// 计算字符串中的字符数量。
    /// </summary>
    /// <param name="target">目标字符串。</param>
    /// <returns>字符串的字符数量。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <see langword="null"/>。</exception>
    public static int GetWordCountFromString(string target)
    {
        ArgumentNullException.ThrowIfNull(target);

        int wordCount = target.EnumerateRunes().Count(Rune.IsLetterOrDigit);

        return wordCount;
    }
}
