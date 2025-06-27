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
    public VolumeInfo GetLatestVolumeInfo()
    {
        List<DirectoryInfo> volumeFolderInfos = GetAllVolumeFolders();
        volumeFolderInfos.Sort(volumeDirectoryOrderComparer);
        volumeFolderInfos.Reverse();
        DirectoryInfo latestVolumeFolderInfo = volumeFolderInfos.First();

        return GetTargetVolumeInfoCore(latestVolumeFolderInfo);
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

    private List<DirectoryInfo> GetAllVolumeFolders()
    {
        string webRootPath = environment.WebRootPath;
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        List<DirectoryInfo> volumeFolderInfos = [.. postsDirectoryInfo.EnumerateDirectories()];
        return volumeFolderInfos;
    }

    private static VolumeInfo GetTargetVolumeInfoCore(DirectoryInfo volumeFolderInfo)
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

            Uri uri = new($"/posts/{volumeDirectoryName}/{Path.ChangeExtension(file.Name, ".html")}", UriKind.Relative);

            volumeDirectories[uri] = articleInfo;
        }

        return new(volumeName, volumeDirectoryName, volumeDirectories);
    }
}

internal static partial class VolumeInfoHelperExtensions
{
    public static void GenerateLatestVolumeInfoJson(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("AnEoT.Vintage.VolumeInfoGenerator");

        IWebHostEnvironment environment = host.Services.GetRequiredService<IWebHostEnvironment>();
        VolumeInfoHelper helper = host.Services.GetRequiredService<VolumeInfoHelper>();

        string jsonSavePath = Path.Combine(environment.WebRootPath, "latest-volume.json");
        using FileStream jsonSaveStream = File.Create(jsonSavePath);
        VolumeInfo latestVolumeInfo = helper.GetLatestVolumeInfo();

        JsonSerializer.Serialize(jsonSaveStream, latestVolumeInfo);
        logger.LogJsonGenerated(jsonSavePath);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "已生成最新期刊的信息：{savePath}")]
    private static partial void LogJsonGenerated(this ILogger logger, string savePath);
}