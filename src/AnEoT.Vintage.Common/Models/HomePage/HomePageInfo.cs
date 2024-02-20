// 这里面的有些属性是不需要的，但是仍然保留，不然 YAML 序列化器会炸

using System.Linq;

namespace AnEoT.Vintage.Common.Models.HomePage;

/// <summary>
/// 表示网站首页信息的结构
/// </summary>
public struct HomePageInfo : IEquatable<HomePageInfo>
{
    /// <summary>
    /// 指示是否为主页的值
    /// </summary>
    public bool Home { get; set; }

    /// <summary>
    /// 页面布局
    /// </summary>
    public string Layout { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 首页的主标题
    /// </summary>
    public string HeroText { get; set; }

    /// <summary>
    /// 指示信息是否全屏展示的值
    /// </summary>
    public bool HeroFullScreen { get; set; }

    /// <summary>
    /// 首页的副标题（抬头画师）
    /// </summary>
    public string Tagline { get; set; }

    /// <summary>
    /// 版头主题色
    /// </summary>
    public string HeroAlt { get; set; }

    /// <summary>
    /// 要显示的项目
    /// </summary>
    public IEnumerable<HomePageProjectsItem> Projects { get; set; }

    /// <summary>
    /// 页脚文本
    /// </summary>
    public string Footer { get; set; }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is HomePageInfo info && Equals(info);
    }

    /// <inheritdoc/>
    public readonly bool Equals(HomePageInfo other)
    {
        return Home == other.Home &&
               Layout == other.Layout &&
               Icon == other.Icon &&
               Title == other.Title &&
               HeroText == other.HeroText &&
               HeroFullScreen == other.HeroFullScreen &&
               Tagline == other.Tagline &&
               HeroAlt == other.HeroAlt &&
               Projects.SequenceEqual(other.Projects) &&
               Footer == other.Footer;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Home);
        hash.Add(Layout);
        hash.Add(Icon);
        hash.Add(Title);
        hash.Add(HeroText);
        hash.Add(HeroFullScreen);
        hash.Add(Tagline);
        hash.Add(HeroAlt);
        hash.Add(Projects);
        hash.Add(Footer);
        return hash.ToHashCode();
    }

    /// <summary>
    /// 确定两个 <see cref="HomePageInfo"/> 实例是否相等
    /// </summary>
    /// <param name="left">第一个 <see cref="HomePageInfo"/> 实例</param>
    /// <param name="right">第二个 <see cref="HomePageInfo"/> 实例</param>
    /// <returns>指示二者是否相等的值</returns>
    public static bool operator ==(HomePageInfo left, HomePageInfo right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 确定两个 <see cref="HomePageInfo"/> 实例是否不同
    /// </summary>
    /// <param name="left">第一个 <see cref="HomePageInfo"/> 实例</param>
    /// <param name="right">第二个 <see cref="HomePageInfo"/> 实例</param>
    /// <returns>指示二者是否不同的值</returns>
    public static bool operator !=(HomePageInfo left, HomePageInfo right)
    {
        return !(left == right);
    }
}
