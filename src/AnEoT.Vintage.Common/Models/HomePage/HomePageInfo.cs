using System.Linq;

namespace AnEoT.Vintage.Common.Models.HomePage;

//有些属性是不需要的，但是仍然保留，不然YAML序列化器会炸
/// <summary>
/// 表示网站首页信息的结构
/// </summary>
public struct HomePageInfo : IEquatable<HomePageInfo>
{
    /// <summary>
    /// 指示是否为主页
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
    /// <!--英雄文本（迫真）-->
    /// </summary>
    public string HeroText { get; set; }
    /// <summary>
    /// <!--英雄全屏（乐）-->
    /// </summary>
    public bool HeroFullScreen { get; set; }
    /// <summary>
    /// 抬头画师
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
    /// <!-- 摸鱼~摸鱼~我就是不写文档注释~咕噜咕噜~和缪缪一起摸鱼~ -->
    /// </summary>
    public string Footer { get; set; }

    public override readonly bool Equals(object? obj)
    {
        return obj is HomePageInfo info && Equals(info);
    }

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

    public static bool operator ==(HomePageInfo left, HomePageInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HomePageInfo left, HomePageInfo right)
    {
        return !(left == right);
    }
}
