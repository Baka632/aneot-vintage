using System.Linq;

namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示期刊信息的结构
/// </summary>
/// <remarks>
/// 使用指定的参数构造 <see cref="VolumeInfo"/> 的新实例
/// </remarks>
/// <param name="name">期刊名称</param>
/// <param name="articles">Key 为文章标题，Value 为文章 Uri 的期刊文章信息字典</param>
public readonly struct VolumeInfo(string name, IReadOnlyDictionary<string, string> articles) : IEquatable<VolumeInfo>
{
    /// <summary>
    /// 当前期刊的名称
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Key为文章标题，Value为文章Uri，包含期刊文章信息的字典
    /// </summary>
    public IReadOnlyDictionary<string, string> Articles { get; } = articles ?? throw new ArgumentNullException(nameof(articles));

    public override readonly bool Equals(object? obj)
    {
        return obj is VolumeInfo info && Equals(info);
    }

    public readonly bool Equals(VolumeInfo other)
    {
        return Name.Equals(other.Name, StringComparison.Ordinal) && Articles.SequenceEqual(other.Articles);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Articles);
    }

    public static bool operator ==(VolumeInfo left, VolumeInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VolumeInfo left, VolumeInfo right)
    {
        return !(left == right);
    }
}