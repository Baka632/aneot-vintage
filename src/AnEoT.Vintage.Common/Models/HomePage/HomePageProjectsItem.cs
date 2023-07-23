namespace AnEoT.Vintage.Common.Models.HomePage;

/// <summary>
/// 表示首页显示项信息的结构
/// </summary>
public struct HomePageProjectsItem
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
}
