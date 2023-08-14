namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为文章分类相关功能提供帮助的类
/// </summary>
public static class CategoryHelper
{
    private static readonly List<string> _categories = new(10);
    private static readonly Dictionary<string, List<string>> _categoryArticleMapping = new(500);

    /// <summary>
    /// 获取全部文章分类
    /// </summary>
    /// <returns>包含文章分类的<see cref="IEnumerable{String}"/></returns>
    public static IEnumerable<string> GetAllCategories(string webRootPath)
    {
        if (_categories.Any())
        {
            return _categories;
        }

        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //添加特定期刊下的文章的信息
            foreach (FileInfo file in volDirInfo.EnumerateFiles("*.md"))
            {
                string markdown = File.ReadAllText(file.FullName);
                ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

                if (articleInfo.Article && articleInfo.Category is not null && articleInfo.Category.Any())
                {
                    foreach (string categoryString in articleInfo.Category)
                    {
                        if (_categories.Contains(categoryString) is not true)
                        {
                            _categories.Add(categoryString);
                        }
                    }
                }
            }
        }

        return _categories;
    }

    /// <summary>
    /// 获取文章分类与文章相对地址的映射
    /// </summary>
    /// <param name="webRootPath"></param>
    /// <returns>一个包含映射的字典</returns>
    public static IDictionary<string, List<string>> GetCategoryToArticleMapping(string webRootPath)
    {
        if (_categoryArticleMapping.Any())
        {
            return _categoryArticleMapping;
        }

        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //添加特定期刊下的文章的信息
            foreach (FileInfo file in volDirInfo.EnumerateFiles("*.md"))
            {
                string markdown = File.ReadAllText(file.FullName);
                ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

                if (articleInfo.Article && articleInfo.Category is not null && articleInfo.Category.Any())
                {
                    string relativePath = Path.GetRelativePath(webRootPath, file.FullName).Replace('\\','/');

                    foreach (string categoryString in articleInfo.Category)
                    {
                        if (_categoryArticleMapping.TryGetValue(categoryString, out List<string>? list))
                        {
                            list.Add(relativePath);
                        }
                        else
                        {
                            list = new List<string>(50)
                            {
                                relativePath
                            };

                            _categoryArticleMapping.Add(categoryString, list);
                        }
                    }
                }
            }
        }

        return _categoryArticleMapping;
    }
}
