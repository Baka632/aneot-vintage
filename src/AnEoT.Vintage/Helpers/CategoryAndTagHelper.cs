namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为文章分类及标签的相关功能提供帮助的类。
/// </summary>
public class CategoryAndTagHelper(CommonValuesHelper commonValues)
{
    private static readonly List<string> _categories = new(10);
    private static readonly List<string> _tags = new(100);
    private static readonly Dictionary<string, List<string>> _categoryArticleMapping = new(500);
    private static readonly Dictionary<string, List<string>> _tagArticleMapping = new(500);

    /// <summary>
    /// 获取全部文章分类。
    /// </summary>
    /// <returns>包含文章分类的 <see cref="IEnumerable{String}"/>。</returns>
    public IEnumerable<string> GetAllCategories()
    {
        if (_categories.Count != 0)
        {
            return _categories;
        }

        string webRootPath = commonValues.WebRootPath;
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
    /// 获取全部文章标签。
    /// </summary>
    /// <returns>包含文章标签的 <see cref="IEnumerable{String}"/>。</returns>
    public IEnumerable<string> GetAllTags()
    {
        if (_tags.Count != 0)
        {
            return _tags;
        }

        string webRootPath = commonValues.WebRootPath;
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //添加特定期刊下的文章的信息
            foreach (FileInfo file in volDirInfo.EnumerateFiles("*.md"))
            {
                string markdown = File.ReadAllText(file.FullName);
                ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

                if (articleInfo.Tag is not null && articleInfo.Tag.Any())
                {
                    foreach (string tagString in articleInfo.Tag)
                    {
                        if (_tags.Contains(tagString) is not true)
                        {
                            _tags.Add(tagString);
                        }
                    }
                }
            }
        }

        return _tags;
    }

    /// <summary>
    /// 获取文章分类与文章相对地址的映射。
    /// </summary>
    /// <returns>一个包含映射的字典。</returns>
    public IDictionary<string, List<string>> GetCategoryToArticleMapping()
    {
        if (_categoryArticleMapping.Count != 0)
        {
            return _categoryArticleMapping;
        }

        string webRootPath = commonValues.WebRootPath;
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
    
    /// <summary>
    /// 获取文章标签与文章相对地址的映射。
    /// </summary>
    /// <returns>一个包含映射的字典。</returns>
    public IDictionary<string, List<string>> GetTagToArticleMapping()
    {
        if (_tagArticleMapping.Count != 0)
        {
            return _tagArticleMapping;
        }

        string webRootPath = commonValues.WebRootPath;
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));
        foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories())
        {
            //添加特定期刊下的文章的信息
            foreach (FileInfo file in volDirInfo.EnumerateFiles("*.md"))
            {
                string markdown = File.ReadAllText(file.FullName);
                ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

                if (articleInfo.Tag is not null && articleInfo.Tag.Any())
                {
                    string relativePath = Path.GetRelativePath(webRootPath, file.FullName).Replace('\\','/');

                    foreach (string tagString in articleInfo.Tag)
                    {
                        if (_tagArticleMapping.TryGetValue(tagString, out List<string>? list))
                        {
                            list.Add(relativePath);
                        }
                        else
                        {
                            list = new List<string>(50)
                            {
                                relativePath
                            };

                            _tagArticleMapping.Add(tagString, list);
                        }
                    }
                }
            }
        }

        return _tagArticleMapping;
    }
}
