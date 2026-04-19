using System.Globalization;
using System.Text;

namespace AnEoT.Vintage.Models.VueComponentAbstractions;

/// <summary>
/// 期刊信息元素。
/// </summary>
public static class VolumeInfo
{
    /// <summary>
    /// 元素标签名。
    /// </summary>
    public const string TagName = "VOLUMEINFO";

    private readonly static CompositeFormat LatestTitleTemplateFormat = CompositeFormat.Parse("""
        <p class="centering" style="font-size: larger;">
            <strong>{0}: {1}</strong>
        </p>
        """);

    private readonly static CompositeFormat LatestCoverTemplateFormat = CompositeFormat.Parse("""
        <div class="cover-container">
            <img src="{0}" alt="封面图像">
        </div>
        """);

    private readonly static CompositeFormat LatestLinkTemplateFormat = CompositeFormat.Parse("""
        <a href="{0}">
            <strong>在线阅读</strong>
        </a>
        """);

    /// <summary>
    /// 获取 <see cref="VolumeInfo"/> 的 HTML。
    /// </summary>
    /// <param name="type">表示显示内容的 <see cref="VolumeInfoType"/>。</param>
    /// <param name="webRootPath">指示“wwwroot”文件夹的路径。</param>
    /// <param name="convertWebP">指示是否转换 WebP 的值。</param>
    /// <returns>构造好的 <see cref="VolumeInfo"/> 的 HTML。</returns>
    public static string GetHtml(VolumeInfoType type, string webRootPath, bool convertWebP)
    {
        return type switch
        {
            VolumeInfoType.LatestTitle => GetLatestTitleHtml(webRootPath),
            VolumeInfoType.LatestCover => GetLatestCoverHtml(webRootPath, convertWebP),
            VolumeInfoType.LatestLink => GetLatestLinkHtml(webRootPath),
            VolumeInfoType.Volumes => GetVolumesHtml(webRootPath, convertWebP),
            _ => throw new NotImplementedException("尚未实现新 VolumeInfoType 的类型。"),
        };
    }

    private static string GetVolumesHtml(string webRootPath, bool convertWebP)
    {
        List<DirectoryInfo> directories = GetVolumeFolders(webRootPath);
        string imageExtensions = convertWebP ? "jpg" : "webp";

        // 这里的数字是根据实际生成文本长度来估算的 StringBuilder 大小。
        StringBuilder builder = new(directories.Count * 180 + 310);

        builder.AppendLine("<table>");
        builder.AppendLine("<tbody>");

        int index = 0;
        int count = 0;
        IEnumerable<IGrouping<int, DirectoryInfo>> groups = directories.GroupBy(info =>
        {
            if (count != 0 && count % 3 == 0)
            {
                count = 1;
                index++;
            }
            else
            {
                count++;
            }

            return index;
        });

        foreach (IGrouping<int, DirectoryInfo> group in groups)
        {
            WriteTableContent(builder, group, (sb, volDir) =>
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"""<img src="/posts/{volDir.Name}/res/cover.{imageExtensions}" />""");
            });

            WriteTableContent(builder, group, (sb, volDir) =>
            {
                string rawTitle = GetVolumeTitle(volDir);
                string title = $"{ volDir.Name}: {rawTitle.Replace("-", "<br/>")}";

                sb.AppendLine(CultureInfo.InvariantCulture, $"""<a href="/posts/{volDir.Name}/">{title}</a>""");
            }, "text-align: center;");
        }

        builder.AppendLine("</table>");
        builder.AppendLine("</tbody>");

        return builder.ToString();
    }

    private static void WriteTableContent(StringBuilder builder, IGrouping<int, DirectoryInfo> group, Action<StringBuilder, DirectoryInfo> writeAction, string? tdStyle = null)
    {
        builder.AppendLine("<tr>");
        foreach (DirectoryInfo volumeDir in group)
        {
            if (string.IsNullOrWhiteSpace(tdStyle))
            {
                builder.AppendLine("<td>");
            }
            else
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"""<td style="{tdStyle}">""");
            }

            writeAction?.Invoke(builder, volumeDir);
            builder.AppendLine("</td>");
        }
        builder.AppendLine("</tr>");
    }

    private static string GetLatestLinkHtml(string webRootPath)
    {
        DirectoryInfo latestVolumeDir = GetLatestVolumeDirectory(webRootPath);
        string link = $"/posts/{latestVolumeDir.Name}/";
        return string.Format(CultureInfo.InvariantCulture, LatestLinkTemplateFormat, link);
    }

    private static string GetLatestCoverHtml(string webRootPath, bool convertWebP)
    {
        string imageExtensions = convertWebP ? "jpg" : "webp";

        DirectoryInfo latestVolumeDir = GetLatestVolumeDirectory(webRootPath);
        string coverUri = $"/posts/{latestVolumeDir.Name}/res/cover.{imageExtensions}";

        return string.Format(CultureInfo.InvariantCulture, LatestCoverTemplateFormat, coverUri);
    }

    private static string GetLatestTitleHtml(string webRootPath)
    {
        DirectoryInfo latestVolumeDir = GetLatestVolumeDirectory(webRootPath);
        string title = GetVolumeTitle(latestVolumeDir);

        return string.Format(CultureInfo.InvariantCulture, LatestTitleTemplateFormat, latestVolumeDir.Name, title);
    }

    private static string GetVolumeTitle(DirectoryInfo volumeDir)
    {
        FileInfo readme = volumeDir.EnumerateFiles("*.md")
                    .Single(info => info.Name.Equals("README.md", StringComparison.OrdinalIgnoreCase));

        string markdown = File.ReadAllText(readme.FullName);
        ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

        string title = articleInfo.Title;
        return title;
    }

    private static DirectoryInfo GetLatestVolumeDirectory(string webRootPath)
    {
        List<DirectoryInfo> volDirInfos = GetVolumeFolders(webRootPath);
        volDirInfos.Reverse();

        DirectoryInfo latestVolumeDir = volDirInfos.First();
        return latestVolumeDir;
    }

    private static List<DirectoryInfo> GetVolumeFolders(string webRootPath)
    {
        // 获取 posts 文件夹的信息。
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));

        // 反向读取文件夹，以获取到最新的期刊。
        List<DirectoryInfo> volDirInfos = [.. postsDirectoryInfo.EnumerateDirectories()];
        volDirInfos.Sort(new VolumeDirectoryOrderComparer());
        return volDirInfos;
    }
}
