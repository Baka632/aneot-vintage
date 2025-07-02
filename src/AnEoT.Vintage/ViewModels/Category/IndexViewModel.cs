namespace AnEoT.Vintage.ViewModels.Category;

/// <summary>
/// 为 Category/Index 页面提供信息的模型。
/// </summary>
public class IndexViewModel
{
    /// <summary>
    /// 获取文章分类与文章相对路径的映射。
    /// </summary>
    public IDictionary<string, List<string>> CategoryToArticleRelativePathMapping { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="IndexViewModel"/> 的新实例。
    /// </summary>
    /// <param name="categoryToArticleRelativePathMapping">文章分类与文章相对路径的映射。</param>
    public IndexViewModel(IDictionary<string, List<string>> categoryToArticleRelativePathMapping)
    {
        CategoryToArticleRelativePathMapping = categoryToArticleRelativePathMapping ?? throw new ArgumentNullException(nameof(categoryToArticleRelativePathMapping));
    }
}
