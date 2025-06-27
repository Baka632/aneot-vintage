using System.Globalization;
using System.Text;
using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 生成磁贴信息的类
/// </summary>
public static class TileHelper
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
    /// 生成磁贴的 XML
    /// </summary>
    /// <param name="baseUri">基 Uri</param>
    /// <param name="webRootPath">“wwwroot”文件夹所在路径</param>
    public static void GenerateTileXml(string baseUri, string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            throw new ArgumentException($"“{nameof(webRootPath)}”不能为 null 或空白。", nameof(webRootPath));
        }

        Console.WriteLine("正在生成磁贴信息...");

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

        Uri baseUriInstance = new(baseUri);

        foreach (DirectoryInfo volDirInfo in targetDirectories)
        {
            string fileName = fileNames.Pop();

            FileInfo readmeFile = volDirInfo.EnumerateFiles("*.md")
                                                 .First(file => file.Name.Equals("README.md", StringComparison.OrdinalIgnoreCase));
            string markdown = File.ReadAllText(readmeFile.FullName);
            ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

            string volumeTypeIndicator = fileName == firstItem ? "最新一期" : "先前期刊";
            string title = articleInfo.Title;
            string coverImage = new Uri(baseUriInstance, $"images/tile/{Path.ChangeExtension(fileName, ".jpg")}").ToString();

            string xml = string.Format(CultureInfo.InvariantCulture, tileTemplate, coverImage, volumeTypeIndicator, title);

            using StreamWriter textWriter = File.CreateText(Path.Combine(tilesDirectoryInfo.FullName, fileName));
            textWriter.Write(xml);
        }

        Console.WriteLine("磁贴信息生成完成！");
    }
}
