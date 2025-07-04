namespace AnEoT.Vintage.ViewModels.Category;

/// <summary>
/// 为 Category/Detail 页提供数据的类。
/// </summary>
public sealed class DetailViewModel
{
    /// <summary>
    /// 获取当前页要查看的分类。
    /// </summary>
    public string Category { get; }
    /// <summary>
    /// 获取分类所属的文章链接列表。
    /// </summary>
    public IEnumerable<string> ArticleUris { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="DetailViewModel"/> 的新实例。
    /// </summary>
    /// <param name="category">当前页要查看的分类。</param>
    /// <param name="articlesUris">分类所属的文章链接列表。</param>
    public DetailViewModel(string category, IEnumerable<string> articlesUris)
    {
        Category = category;
        ArticleUris = articlesUris;
    }
}
