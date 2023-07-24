namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为"wwwroot"文件夹内文件的相关操作提供帮助方法
/// </summary>
public static class WebRootFileHelper
{
    /// <summary>
    /// 将"wwwroot"文件夹内对静态网页所必需的文件复制到指定的目录中
    /// </summary>
    /// <param name="webRootPath">“wwwroot”文件夹所在路径</param>
    /// <param name="outputPath">静态网页的输出路径</param>
    /// <param name="convertWebpToJpg">指示是否将 WebP 图像转换为 JPG 图像的值</param>
    public static void CopyFilesToStaticWebSiteOutputPath(string webRootPath, string outputPath, bool convertWebpToJpg)
    {
        DirectoryInfo wwwRootDirectory = new(webRootPath);
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));

        #region 复制特定期刊的资源文件
        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //复制特定期刊的静态文件
            DirectoryInfo resDirInfo = new(Path.Combine(volDirInfo.FullName, "res"));
            if (resDirInfo.Exists)
            {
                CopyDirectory(resDirInfo, Path.Combine(outputPath, "posts", volDirInfo.Name, "res"), true, convertWebpToJpg);
            }
        }
        #endregion

        #region 复制wwwroot下的文件夹与文件
        //复制wwwroot下的文件夹（无posts文件夹）
        IEnumerable<DirectoryInfo> directories = wwwRootDirectory.EnumerateDirectories()
            .Where(dir => !dir.Name.Contains("posts"));

        foreach (DirectoryInfo item in directories)
        {
            CopyDirectory(item, Path.Combine(outputPath, item.Name), true, convertWebpToJpg);
        }

        //复制非md扩展名的文件
        IEnumerable<FileInfo> fileInfos = wwwRootDirectory.EnumerateFiles().Where(file => file.Extension != ".md");
        foreach (FileInfo fileInfo in fileInfos)
        {
            string targetFilePath = Path.Combine(outputPath, fileInfo.Name);
            fileInfo.CopyTo(targetFilePath, true);
        }
        #endregion
    }

    /// <summary>
    /// 复制目录
    /// </summary>
    /// <param name="sourceDir">原目录信息</param>
    /// <param name="destinationDir">目标目录路径</param>
    /// <param name="recursive">指示是否复制子目录的值</param>
    /// <param name="convertWebp">指示是否对文件夹中的 WebP 图像进行转换的值</param>
    /// <exception cref="DirectoryNotFoundException">当找不到指定的文件夹时抛出</exception>
    private static void CopyDirectory(DirectoryInfo sourceDir, string destinationDir, bool recursive, bool convertWebp)
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
            if (convertWebp && file.Extension.Equals(".webp", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"正在转换：{file.Name}");

                string targetFilePath = Path.Combine(destinationDir, Path.ChangeExtension(file.Name, ".jpg"));
                using Image image = Image.Load(file.FullName);
                image.Mutate(x => x.BackgroundColor(Color.White));
                image.SaveAsJpeg(targetFilePath);
            }
            else
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }
        }

        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir, newDestinationDir, true, convertWebp);
            }
        }
    }
}
