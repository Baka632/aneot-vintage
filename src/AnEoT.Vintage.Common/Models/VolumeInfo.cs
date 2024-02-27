using System.Linq;

namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示期刊信息的结构
/// </summary>
/// <param name="name">期刊名称</param>
/// <param name="articles">Key 为文章标题，Value 为文章 Uri 的期刊文章信息字典</param>
public readonly struct VolumeInfo(string name, IReadOnlyDictionary<string, string> articles) : IEquatable<VolumeInfo>
{
    /// <summary>
    /// 当前期刊的名称
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Key 为文章标题，Value 为文章 Uri，包含期刊文章信息的字典
    /// </summary>
    public IReadOnlyDictionary<string, string> Articles { get; } = articles ?? throw new ArgumentNullException(nameof(articles));

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is VolumeInfo info && Equals(info);
    }

    /// <inheritdoc/>
    public readonly bool Equals(VolumeInfo other)
    {
        return Name.Equals(other.Name, StringComparison.Ordinal) && Articles.SequenceEqual(other.Articles);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Articles);
    }

    /// <summary>
    /// 确定两个 <see cref="VolumeInfo"/> 实例是否相等
    /// </summary>
    /// <param name="left">第一个 <see cref="VolumeInfo"/> 实例</param>
    /// <param name="right">第二个 <see cref="VolumeInfo"/> 实例</param>
    /// <returns>指示二者是否相等的值</returns>
    public static bool operator ==(VolumeInfo left, VolumeInfo right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 确定两个 <see cref="VolumeInfo"/> 实例是否不同
    /// </summary>
    /// <param name="left">第一个 <see cref="VolumeInfo"/> 实例</param>
    /// <param name="right">第二个 <see cref="VolumeInfo"/> 实例</param>
    /// <returns>指示二者是否不同的值</returns>
    public static bool operator !=(VolumeInfo left, VolumeInfo right)
    {
        return !(left == right);
    }
}