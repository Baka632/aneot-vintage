namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为获取主页信息提供帮助的类
/// </summary>
public static class HomePageHelper
{
    private static readonly VolumeOrderComparer volumeOrderComparer = new();

    /// <summary>
    /// 获取主页信息
    /// </summary>
    /// <param name="webRootPath">"wwwroot"文件夹的路径</param>
    /// <returns>表示主页信息的 <see cref="HomePageInfo"/></returns>
    public static HomePageInfo GetHomePageInfo(string webRootPath)
    {
        FileInfo fileInfo = new(Path.Combine(webRootPath, "Homepage.md"));

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"找不到指定的文件：{fileInfo.FullName}");
        }

        string markdown = File.ReadAllText(fileInfo.FullName);

        HomePageInfo info = MarkdownHelper.GetFromFrontMatter<HomePageInfo>(markdown);

        return info;
    }

    /// <summary>
    /// 获取最新一期期刊文件夹的字符串
    /// </summary>
    /// <param name="webRootPath"></param>
    /// <returns>最新一期期刊文件夹的字符串</returns>
    public static string GetLatestVolume(string webRootPath)
    {
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        List<DirectoryInfo> volDirInfos = new(postsDirectoryInfo.EnumerateDirectories());
        volDirInfos.Sort(volumeOrderComparer);
        volDirInfos.Reverse();
        return volDirInfos.First().Name;
    }
}
