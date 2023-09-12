namespace AnEoT.Vintage.Models;

/// <summary>
/// 为「泰拉广告」提供信息的结构
/// </summary>
public readonly struct FakeAdInfo
{
    /// <summary>
    /// 广告显示文本
    /// </summary>
    public string? AdText { get; init; }
    /// <summary>
    /// 广告帮助信息文本
    /// </summary>
    public string? AdAbout { get; init; }
    /// <summary>
    /// 广告链接
    /// </summary>
    public string? AdLink { get; init; }
    /// <summary>
    /// 广告帮助信息链接
    /// </summary>
    public string? AboutLink { get; init; }
    /// <summary>
    /// 广告图像链接
    /// </summary>
    public string? AdImageLink { get; init; }
}
