namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为获取主页信息提供帮助的类。
/// </summary>
public class HomePageHelper(IWebHostEnvironment environment)
{
    /// <summary>
    /// 获取主页信息。
    /// </summary>
    /// <returns>表示主页信息的 <see cref="HomePageInfo"/>。</returns>
    public HomePageInfo GetHomePageInfo()
    {
        string webRootPath = environment.WebRootPath;

        FileInfo fileInfo = new(Path.Combine(webRootPath, "README.md"));

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"找不到指定的文件：{fileInfo.FullName}");
        }

        string markdown = File.ReadAllText(fileInfo.FullName);

        HomePageInfo info = MarkdownHelper.GetFromFrontMatter<HomePageInfo>(markdown);

        return info;
    }
}
