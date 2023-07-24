using AspNetStatic;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为静态网页生成提供帮助方法
/// </summary>
public static class StaticWebSiteHelper
{
    /// <summary>
    /// 获取用于描述网站内容的<see cref="StaticPagesInfoProvider"/>
    /// </summary>
    /// <returns>描述网站内容的<see cref="StaticPagesInfoProvider"/></returns>
    public static StaticPagesInfoProvider GetStaticPagesInfoProvider(string webRootPath)
    {
        List<PageInfo> pages = new(2000)
        {
            new PageInfo("/"),
            new PageInfo("/posts") { OutFile = Path.Combine("posts","index.html") },
        };

        DirectoryInfo wwwRootDirectory = new(webRootPath);

        #region 第一步：根据wwwroot下的Markdown文件来生成网页内容信息
        foreach (FileInfo item in wwwRootDirectory.EnumerateFiles("*.md"))
        {
            //去掉Homepage.md，因为前面已经间接添加过了
            if (item.Name.Contains("Homepage.md"))
            {
                continue;
            }

            string nameRemovedExtension = item.Name.Replace(".md", string.Empty);
            pages.Add(new($"/{nameRemovedExtension}"));
        }
        #endregion

        #region 第二步：根据wwwroot\posts下的文件与文件夹来生成网页内容信息
        //获取posts文件夹的信息
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));

        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //添加各期刊页面的信息
            PageInfo info = new($"/posts/{volDirInfo.Name}")
            {
                OutFile = Path.Combine("posts", volDirInfo.Name, "index.html")
            };

            pages.Add(info);

            //添加特定期刊下的文章的信息
            foreach (FileInfo article in volDirInfo.EnumerateFiles("*.md"))
            {
                //去掉README.md，因为前面已经间接添加过了
                if (article.Name.Contains("README.md"))
                {
                    continue;
                }

                string nameRemovedExtension = article.Name.Replace(".md", string.Empty);
                pages.Add(new($"/posts/{volDirInfo.Name}/{nameRemovedExtension}"));
            }
        }
        #endregion

        StaticPagesInfoProvider provider = new(pages);
        return provider;
    }
}
