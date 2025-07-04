namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示期刊信息的结构。
/// </summary>
/// <param name="VolumeName">期刊名称。</param>
/// <param name="VolumeFolderName">期刊文件夹名称。</param>
/// <param name="Articles">Key 为文章相对 <see cref="Uri"/>，Value 为 <see cref="ArticleInfo"/> 实例的期刊文章信息字典。</param>
public record struct VolumeInfo(
    string VolumeName,
    string VolumeFolderName,
    IReadOnlyDictionary<Uri, ArticleInfo> Articles);