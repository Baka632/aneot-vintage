using System.Net;
using AspNetStatic;
using AspNetStatic.Optimizer;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为静态网站生成提供帮助方法。
/// </summary>
public class StaticWebSiteHelper(
    CommonValuesHelper commonValues,
    CategoryAndTagHelper categoryAndTagHelper)
{
    /// <summary>
    /// 获取用于描述网站内容的 <see cref="StaticResourcesInfoProvider"/>。
    /// </summary>
    /// <returns>描述网站内容的 <see cref="StaticResourcesInfoProvider"/>。</returns>
    public StaticResourcesInfoProvider GetStaticResourcesInfo()
    {
        string webRootPath = commonValues.WebRootPath;
        bool convertWebp = commonValues.ConvertWebP;

        string[] excludedFiles = [];
        string[] excludedFolders= [];

        List<ResourceInfoBase> pages = new(2500)
        {
            new PageResource("/"),
            new PageResource("/settings"),
            new PageResource("/installpwa"),
            new CssResource("/AnEoT.Vintage.styles.css"),
        };

        DirectoryInfo wwwRootDirectory = new(webRootPath);

        #region 第一步：根据wwwroot下的文件与文件夹来生成网页内容信息
        List<ResourceInfoBase> wwwRootPages = GetPageInfoFromDirectory(wwwRootDirectory, "/", excludedFolders, excludedFiles, convertWebp);
        pages.AddRange(wwwRootPages);
        #endregion

        #region 第二步：生成分类页与标签页的网页内容信息
        pages.Add(new PageResource("/category")
        {
            OutFile = Path.Combine("category", "index.html")
        });
        pages.Add(new PageResource("/tag")
        {
            OutFile = Path.Combine("tag", "index.html")
        });

        foreach (string category in categoryAndTagHelper.GetAllCategories())
        {
            pages.Add(new PageResource($"/category/{WebUtility.UrlEncode(category)}")
            {
                OutFile = Path.Combine("category", category, "index.html")
            });
        }

        foreach (string tag in categoryAndTagHelper.GetAllTags())
        {
            pages.Add(new PageResource($"/tag/{WebUtility.UrlEncode(tag)}")
            {
                OutFile = Path.Combine("tag", tag, "index.html")
            });
        }
        #endregion

        StaticResourcesInfoProvider provider = new(pages);
        return provider;
    }

    private static List<ResourceInfoBase> GetPageInfoFromDirectory(DirectoryInfo baseDirectory, string baseUri, IEnumerable<string>? excludedFolderName, IEnumerable<string>? excludedFileName, bool convertWebp)
    {
        excludedFolderName ??= [];
        excludedFileName ??= [];

        List<ResourceInfoBase> pages = new(100);

        foreach (FileInfo file in baseDirectory.EnumerateFiles().Where(fileInfo => !excludedFileName.Contains(fileInfo.Name)))
        {
            string name = file.Name;
            string extensions = file.Extension;

            if (name.Equals("README.md", StringComparison.OrdinalIgnoreCase))
            {
                pages.Add(new PageResource(baseUri)
                {
                    OutFile = Path.Combine(baseUri, "index.html")
                });
            }
            else if (extensions.Equals(".md", StringComparison.OrdinalIgnoreCase))
            {
                string nameRemovedExtension = name.Replace(".md", string.Empty);
                pages.Add(new PageResource(Path.Combine(baseUri, nameRemovedExtension).Replace('\\', '/')));
            }
            else if (extensions.Equals(".js", StringComparison.OrdinalIgnoreCase))
            {
                pages.Add(new JsResource(Path.Combine(baseUri, name).Replace('\\', '/')));
            }
            else if (extensions.Equals(".css", StringComparison.OrdinalIgnoreCase))
            {
                pages.Add(new CssResource(Path.Combine(baseUri, name).Replace('\\', '/')));
            }
            else
            {
                bool isWebpFile = extensions.Equals(".webp", StringComparison.OrdinalIgnoreCase);

                BinResource binRes = new(Path.Combine(baseUri, name).Replace('\\', '/'))
                {
                    OutFile = convertWebp && isWebpFile
                        ? Path.ChangeExtension(Path.Combine(baseUri, name), ".jpg")
                        : Path.Combine(baseUri, name),
                    OptimizationType = isWebpFile
                        ? OptimizationType.Bin
                        : OptimizationType.None
                };

                pages.Add(binRes);
            }
        }

        foreach (DirectoryInfo directory in baseDirectory.EnumerateDirectories().Where(dirInfo => !excludedFolderName.Contains(dirInfo.Name)))
        {
            List<ResourceInfoBase> infos = GetPageInfoFromDirectory(directory, Path.Combine(baseUri, directory.Name).Replace('\\', '/'), null, null, convertWebp);
            pages.AddRange(infos);
        }

        return pages;
    }
}
