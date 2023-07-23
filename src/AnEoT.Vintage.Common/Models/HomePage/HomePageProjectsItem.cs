namespace AnEoT.Vintage.Common.Models.HomePage;

/// <summary>
/// 表示首页显示项信息的结构
/// </summary>
public struct HomePageProjectsItem : IEquatable<HomePageProjectsItem>
{
    /// <summary>
    /// 类型图标
    /// </summary>
    public string Icon { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }
    /// <summary>
    /// 指向的链接
    /// </summary>
    public string Link { get; set; }

    public override readonly bool Equals(object? obj)
    {
        return obj is HomePageProjectsItem item && Equals(item);
    }

    public readonly bool Equals(HomePageProjectsItem other)
    {
        return Icon == other.Icon &&
               Name == other.Name &&
               Desc == other.Desc &&
               Link == other.Link;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Icon, Name, Desc, Link);
    }

    public static bool operator ==(HomePageProjectsItem left, HomePageProjectsItem right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HomePageProjectsItem left, HomePageProjectsItem right)
    {
        return !(left == right);
    }
}
