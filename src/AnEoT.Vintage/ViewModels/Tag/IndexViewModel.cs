namespace AnEoT.Vintage.ViewModels.Tag;

/// <summary>
/// 为 Tag/Index 页面提供信息的模型
/// </summary>
public class IndexViewModel
{
    /// <summary>
    /// 获取文章标签与文章相对路径的映射
    /// </summary>
    public IDictionary<string, List<string>> TagToArticleRelativePathMapping { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="IndexViewModel"/> 的新实例
    /// </summary>
    /// <param name="tagToArticleRelativePathMapping">文章分类与文章相对路径的映射</param>
    public IndexViewModel(IDictionary<string, List<string>> tagToArticleRelativePathMapping)
    {
        TagToArticleRelativePathMapping = tagToArticleRelativePathMapping ?? throw new ArgumentNullException(nameof(tagToArticleRelativePathMapping));
    }
}
