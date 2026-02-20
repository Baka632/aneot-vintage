using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为提供期刊相关信息提供帮助的类。
/// </summary>
public class VolumeInfoHelper(IWebHostEnvironment environment, VolumeDirectoryOrderComparer volumeDirectoryOrderComparer)
{
    /// <summary>
    /// 获取最新一期期刊的信息。
    /// </summary>
    /// <param name="fillArticleQuote">指示是否为每篇文章的信息填充文章摘要的值。</param>
    public VolumeInfo GetLatestVolumeInfo(bool fillArticleQuote = false)
    {
        List<DirectoryInfo> volumeFolderInfos = GetAllVolumeFolders();
        volumeFolderInfos.Sort(volumeDirectoryOrderComparer);
        volumeFolderInfos.Reverse();
        DirectoryInfo latestVolumeFolderInfo = volumeFolderInfos.First();

        return GetTargetVolumeInfoCore(latestVolumeFolderInfo, fillArticleQuote);
    }

    /// <summary>
    /// 获取指定一期期刊的信息。
    /// </summary>
    /// <param name="volumeFolderName">形如“2025-06”的期刊文件夹名称。</param>
    public VolumeInfo GetTargetVolumeInfo(string volumeFolderName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(volumeFolderName);

        List<DirectoryInfo> volumeFolderInfos = GetAllVolumeFolders();
        DirectoryInfo targetFolder = volumeFolderInfos.Single(info => info.Name.Equals(volumeFolderName, StringComparison.OrdinalIgnoreCase));

        return GetTargetVolumeInfoCore(targetFolder);
    }

    /// <summary>
    /// 尝试获取指定一期期刊的信息。
    /// </summary>
    /// <param name="volumeFolderName">形如“2025-06”的期刊文件夹名称。</param>
    /// <param name="info">若获取成功，则为指定期刊的信息；若失败则为 <see langword="null"/>。</param>
    /// <returns>指示操作是否成功的值。</returns>
    public bool TryGetTargetVolumeInfo(string volumeFolderName, [NotNullWhen(true)] out VolumeInfo? info)
    {
        info = null;

        if (string.IsNullOrWhiteSpace(volumeFolderName))
        {
            return false;
        }

        List<DirectoryInfo> volumeFolderInfos = GetAllVolumeFolders();
        DirectoryInfo? targetFolder = volumeFolderInfos.SingleOrDefault(info => info.Name.Equals(volumeFolderName, StringComparison.OrdinalIgnoreCase));

        if (targetFolder is null || !targetFolder.Exists)
        {
            return false;
        }

        try
        {
            info = GetTargetVolumeInfoCore(targetFolder);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 尝试获取当前期刊的上一期期刊。
    /// </summary>
    /// <param name="currentInfo">当前期刊的期刊信息。</param>
    /// <param name="formerInfo">此期刊的上一期期刊，若获取失败则返回 <see langword="null"/>。</param>
    /// <returns>指示操作是否成功的值。</returns>
    public bool TryGetFormerVolumeInfo(VolumeInfo currentInfo, [NotNullWhen(true)] out VolumeInfo? formerInfo)
    {
        formerInfo = GetFormerOrNextVolumeInfoCore(currentInfo, true);
        return formerInfo != null;
    }

    /// <summary>
    /// 尝试获取当前期刊的下一期期刊。
    /// </summary>
    /// <param name="currentInfo">当前期刊的期刊信息。</param>
    /// <param name="nextInfo">此期刊的下一期期刊，若获取失败则返回 <see langword="null"/>。</param>
    /// <returns>指示操作是否成功的值。</returns>
    public bool TryGetNextVolumeInfo(VolumeInfo currentInfo, [NotNullWhen(true)] out VolumeInfo? nextInfo)
    {
        nextInfo = GetFormerOrNextVolumeInfoCore(currentInfo, false);
        return nextInfo != null;
    }

    private VolumeInfo? GetFormerOrNextVolumeInfoCore(VolumeInfo currentInfo, bool isFormer)
    {
        List<DirectoryInfo> volumeFolderInfos = GetAllVolumeFolders();
        volumeFolderInfos.Sort(volumeDirectoryOrderComparer);

        DirectoryInfo? currentInfoDir = volumeFolderInfos.FirstOrDefault(dir => dir.Name == currentInfo.VolumeFolderName);
        if (currentInfoDir is null)
        {
            return null;
        }

        int currentInfoDirIndex = volumeFolderInfos.FindIndex(info => info.FullName == currentInfoDir.FullName);
        int targetInfoDirIndex = isFormer ? currentInfoDirIndex - 1 : currentInfoDirIndex + 1;

        if (targetInfoDirIndex < 0 || targetInfoDirIndex > volumeFolderInfos.Count - 1)
        {
            return null;
        }

        DirectoryInfo targetDirectory = volumeFolderInfos[targetInfoDirIndex];
        try
        {
            return GetTargetVolumeInfoCore(targetDirectory);
        }
        catch
        {
            return null;
        }
    }

    private List<DirectoryInfo> GetAllVolumeFolders()
    {
        string webRootPath = environment.WebRootPath;
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        List<DirectoryInfo> volumeFolderInfos = [.. postsDirectoryInfo.EnumerateDirectories()];
        return volumeFolderInfos;
    }

    private static VolumeInfo GetTargetVolumeInfoCore(DirectoryInfo volumeFolderInfo, bool fillArticleQuote = false)
    {
        ArgumentNullException.ThrowIfNull(volumeFolderInfo);

        string volumeName;
        string volumeDirectoryName;
        Dictionary<Uri, ArticleInfo> volumeDirectories = [];
        
        volumeDirectoryName = volumeFolderInfo.Name;

        IEnumerable<FileInfo> volumeFiles = volumeFolderInfo.EnumerateFiles("*.md");

        const string readmeFile = "README.md";

        FileInfo volumeReadme = volumeFiles.Single(file => file.Name.Equals(readmeFile, StringComparison.OrdinalIgnoreCase));
        IOrderedEnumerable<FileInfo> articles = volumeFiles
                                .Where(file => !file.Name.Equals(readmeFile, StringComparison.OrdinalIgnoreCase))
                                .Order(new ArticleFileOrderComparer());

        string volumeReadmeMarkdown = File.ReadAllText(volumeReadme.FullName);
        ArticleInfo volumeReadmeInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(volumeReadmeMarkdown);
        volumeName = volumeReadmeInfo.Title;

        foreach (FileInfo file in articles)
        {
            string markdown = File.ReadAllText(file.FullName);
            ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

            if (fillArticleQuote)
            {
                string quoteMd = MarkdownHelper.GetArticleQuote(markdown, true);
                string quote = MarkdownHelper.ToPlainText(quoteMd).Trim();

                if (!string.IsNullOrWhiteSpace(quote))
                {
                    articleInfo = articleInfo with { ArticleQuote = quote };
                }
            }

            Uri uri = new($"/posts/{volumeDirectoryName}/{Path.ChangeExtension(file.Name, ".html")}", UriKind.Relative);

            volumeDirectories[uri] = articleInfo;
        }

        return new(volumeName, volumeDirectoryName, volumeDirectories);
    }
}

/// <summary>
/// 为 <see cref="VolumeInfoHelper"/> 提供扩展方法的类。
/// </summary>
public static partial class VolumeInfoHelperExtensions
{
    private const string LoggerName = "AnEoT.Vintage.VolumeInfoGenerator";

    /// <summary>
    /// 生成最新期刊信息的 JSON 文件。
    /// </summary>
    /// <param name="host">.NET 通用主机。</param>
    /// <returns>完成操作后的 <see cref="IHost"/>。</returns>
    public static IHost GenerateLatestVolumeInfoJson(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger(LoggerName);

        IWebHostEnvironment environment = host.Services.GetRequiredService<IWebHostEnvironment>();
        VolumeInfoHelper helper = host.Services.GetRequiredService<VolumeInfoHelper>();

        string jsonSavePath = Path.Combine(environment.WebRootPath, "latest-volume.json");
        using FileStream jsonSaveStream = File.Create(jsonSavePath);
        VolumeInfo latestVolumeInfo = helper.GetLatestVolumeInfo(true);

        JsonSerializer.Serialize(jsonSaveStream, latestVolumeInfo);
        logger.LogJsonGenerated(jsonSavePath);

        return host;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "已生成最新期刊的信息：{savePath}")]
    private static partial void LogJsonGenerated(this ILogger logger, string savePath);
}