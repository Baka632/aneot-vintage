using AspNetStatic;

namespace AnEoT.Vintage.Helper
{
    /// <summary>
    /// 为静态网页生成提供帮助方法
    /// </summary>
    public static class StaticWebSiteHelper
    {
        /// <summary>
        /// 获取用于描述网站内容的<see cref="StaticPagesInfoProvider"/>，并且复制必需的文件
        /// </summary>
        /// <param name="webRootPath">“wwwroot”文件夹所在路径</param>
        /// <param name="outputPath">静态网页的输出路径</param>
        /// <returns>描述网站内容的<see cref="StaticPagesInfoProvider"/></returns>
        public static StaticPagesInfoProvider GetStaticPagesInfoProviderAndCopyFiles(string webRootPath, string outputPath)
        {
            List<PageInfo> pages = new(2000)
            {
                new PageInfo("/"),
                new PageInfo("/posts") { OutFile = Path.Combine("posts","index.html") },
                
                //临时解决措施
                new PageInfo("/posts/2023-06/README.md") { OutFile = Path.Combine("posts","2023-06","README.md","index.html") },
                new PageInfo("/download.md") { OutFile = Path.Combine("download.md","index.html") },
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
                //添加期刊页面的信息
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

                //复制特定期刊的静态文件
                DirectoryInfo resDirInfo = new(Path.Combine(Path.Combine(volDirInfo.FullName, "res")));
                if (resDirInfo.Exists)
                {
                    CopyDirectory(resDirInfo, Path.Combine(outputPath, "posts", volDirInfo.Name, "res"), true);
                }
            }
            #endregion

            #region 第三步：复制必需的静态文件
            //复制wwwroot下的文件夹（无posts文件夹）
            IEnumerable<DirectoryInfo> directories = wwwRootDirectory.EnumerateDirectories().Where(dir => !dir.Name.Contains("posts"));
            foreach (DirectoryInfo item in directories)
            {
                CopyDirectory(item, Path.Combine(outputPath, item.Name), true);
            }

            //复制非md扩展名的文件
            IEnumerable<FileInfo> fileInfos = wwwRootDirectory.EnumerateFiles().Where(file => file.Extension != ".md");
            foreach (FileInfo fileInfo in fileInfos)
            {
                string targetFilePath = Path.Combine(outputPath, fileInfo.Name);
                fileInfo.CopyTo(targetFilePath, true);
            }
            #endregion

            StaticPagesInfoProvider provider = new(pages);
            return provider;
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="sourceDir">原目录信息</param>
        /// <param name="destinationDir">目标目录信息</param>
        /// <param name="recursive">指示是否复制子目录的值</param>
        /// <exception cref="DirectoryNotFoundException">当找不到指定的文件夹时抛出</exception>
        private static void CopyDirectory(DirectoryInfo sourceDir, string destinationDir, bool recursive)
        {
            //源代码来自：https://learn.microsoft.com/dotnet/standard/io/how-to-copy-directories

            DirectoryInfo dir = sourceDir;

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"找不到目录: {dir.FullName}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir, newDestinationDir, true);
                }
            }
        }
    }
}
