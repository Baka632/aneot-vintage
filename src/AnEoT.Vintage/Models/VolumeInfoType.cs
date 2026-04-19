namespace AnEoT.Vintage.Models;

/// <summary>
/// 指示 <see cref="VueComponentAbstractions.VolumeInfo"/> 的显示类型。
/// </summary>
public enum VolumeInfoType
{
    /// <summary>
    /// 显示最新期刊标题。
    /// </summary>
    LatestTitle,
    /// <summary>
    /// 显示最新一期期刊封面。
    /// </summary>
    LatestCover,
    /// <summary>
    /// 显示最新一期期刊查看链接。
    /// </summary>
    LatestLink,
    /// <summary>
    /// 显示期刊列表。
    /// </summary>
    Volumes
}
