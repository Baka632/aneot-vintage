using System.Text;
using System.Globalization;
using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 生成磁贴信息的类。
/// </summary>
public class TileGenerationHelper(CommonValuesHelper commonValues)
{
    private static readonly CompositeFormat tileTemplate = CompositeFormat.Parse("""
        <tile>
          <visual version="2">
            <binding template="TileSquare150x150PeekImageAndText02" branding='name'>
              <image id="1" src="{0}" alt="期刊封面"/> <!-- Cover Image -->
              <text id="1">{1}</text> <!-- Latest or former -->
              <text id="2">{2}</text> <!-- Title -->
            </binding>

            <binding template="TileWide310x150PeekImage01" branding='name'>
              <image id="1" src="{0}" alt="期刊封面"/>
              <text id="1">{1}</text>
              <text id="2">{2}</text>
            </binding>

            <binding template="TileSquare310x310ImageAndTextOverlay02" branding='name'>
              <image id="1" src="{0}" alt="期刊封面"/>
              <text id="1">{1}</text>
              <text id="2">{2}</text>
            </binding>
          </visual>
        </tile>
        """);

    /// <summary>
    /// 生成包含磁贴信息的 XML 文件。
    /// </summary>
    /// <returns>生成磁贴 XML 文件的文件夹路径。</returns>
    public string GenerateTileXml()
    {
        string webRootPath = commonValues.WebRootPath;

        // 获取 posts 文件夹的信息
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        // 磁贴信息保存文件夹
        DirectoryInfo tilesDirectoryInfo = new(Path.Combine(webRootPath, "tiles"));

        if (!tilesDirectoryInfo.Exists)
        {
            tilesDirectoryInfo.Create();
        }

        // 反向读取文件夹，以获取到最新的期刊
        List<DirectoryInfo> volDirInfos = [.. postsDirectoryInfo.EnumerateDirectories()];
        volDirInfos.Sort(new VolumeDirectoryOrderComparer());
        volDirInfos.Reverse();

        // 取前三期的期刊
        const int volumeTakeCount = 3;
        const string firstItem = "main.xml";

        Stack<string> fileNames = new();

        for (int i = volumeTakeCount - 1; i >= 1; i--)
        {
            fileNames.Push($"last{i}.xml");
        }

        fileNames.Push(firstItem);

        IEnumerable<DirectoryInfo> targetDirectories = volDirInfos.Take(volumeTakeCount);

        foreach (DirectoryInfo volDirInfo in targetDirectories)
        {
            string fileName = fileNames.Pop();

            FileInfo readmeFile = volDirInfo.EnumerateFiles("*.md")
                                                 .First(file => file.Name.Equals("README.md", StringComparison.OrdinalIgnoreCase));
            string markdown = File.ReadAllText(readmeFile.FullName);
            ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

            string volumeTypeIndicator = fileName == firstItem ? "最新一期" : "先前期刊";
            string title = articleInfo.Title;
            string coverImage = new Uri(commonValues.BaseUri, $"images/tile/{Path.ChangeExtension(fileName, ".jpg")}").ToString();

            string xml = string.Format(CultureInfo.InvariantCulture, tileTemplate, coverImage, volumeTypeIndicator, title);

            using StreamWriter textWriter = File.CreateText(Path.Combine(tilesDirectoryInfo.FullName, fileName));
            textWriter.Write(xml);
        }

        return tilesDirectoryInfo.FullName;
    }
}

/// <summary>
/// 为 <see cref="TileGenerationHelper"/> 提供扩展方法的类。
/// </summary>
public static partial class TileGenerationHelperExtensions
{
    private const string LoggerName = "AnEoT.Vintage.TileGenerator";

    /// <summary>
    /// 生成呈现期刊信息的磁贴信息。
    /// </summary>
    /// <param name="host">.NET 通用主机。</param>
    /// <returns>完成操作后的 <see cref="IHost"/>。</returns>
    public static IHost GenerateTileXml(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger(LoggerName);

        TileGenerationHelper helper = host.Services.GetRequiredService<TileGenerationHelper>();
        string generateFolderPath = helper.GenerateTileXml();

        logger.LogTileXmlGenerated(generateFolderPath);

        return host;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "已在以下文件夹中生成磁贴信息：{tileFolderPath}")]
    private static partial void LogTileXmlGenerated(this ILogger logger, string tileFolderPath);
}
