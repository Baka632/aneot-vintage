namespace AnEoT.Vintage.Common.Models.HomePage;

/// <summary>
/// 表示首页显示项信息的结构。
/// </summary>
public struct HomePageProjectsItem : IEquatable<HomePageProjectsItem>
{
    /// <summary>
    /// 类型图标。
    /// </summary>
    public string Icon { get; set; }
    /// <summary>
    /// 名称。
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 描述。
    /// </summary>
    public string Desc { get; set; }
    /// <summary>
    /// 指向的链接。
    /// </summary>
    public string Link { get; set; }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is HomePageProjectsItem item && Equals(item);
    }

    /// <inheritdoc/>
    public readonly bool Equals(HomePageProjectsItem other)
    {
        return Icon == other.Icon &&
               Name == other.Name &&
               Desc == other.Desc &&
               Link == other.Link;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Icon, Name, Desc, Link);
    }

    /// <summary>
    /// 确定两个 <see cref="HomePageProjectsItem"/> 实例是否相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="HomePageProjectsItem"/> 实例。</param>
    /// <param name="right">第二个 <see cref="HomePageProjectsItem"/> 实例。</param>
    /// <returns>指示二者是否相等的值。</returns>
    public static bool operator ==(HomePageProjectsItem left, HomePageProjectsItem right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 确定两个 <see cref="HomePageProjectsItem"/> 实例是否不同。
    /// </summary>
    /// <param name="left">第一个 <see cref="HomePageProjectsItem"/> 实例。</param>
    /// <param name="right">第二个 <see cref="HomePageProjectsItem"/> 实例。</param>
    /// <returns>指示二者是否不同的值。</returns>
    public static bool operator !=(HomePageProjectsItem left, HomePageProjectsItem right)
    {
        return !(left == right);
    }
}
