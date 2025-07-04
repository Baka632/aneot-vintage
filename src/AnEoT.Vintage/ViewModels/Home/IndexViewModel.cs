namespace AnEoT.Vintage.ViewModels.Home;

/// <summary>
/// 为 Home/Index 页面提供信息的模型。
/// </summary>
public sealed class IndexViewModel
{
    /// <summary>
    /// 获取包含主页面信息的 <see cref="Common.Models.HomePage.HomePageInfo"/>。
    /// </summary>
    public HomePageInfo HomePageInfo { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="IndexViewModel"/> 的新实例。
    /// </summary>
    /// <param name="homePageInfo">主页面信息。</param>
    public IndexViewModel(HomePageInfo homePageInfo)
    {
        HomePageInfo = homePageInfo;
    }
}
