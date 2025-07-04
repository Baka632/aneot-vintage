namespace AnEoT.Vintage.ViewModels.Tag;

/// <summary>
/// 为 Tag/Detail 页提供数据的类。
/// </summary>
public sealed class DetailViewModel
{
    /// <summary>
    /// 获取当前页要查看的标签。
    /// </summary>
    public string Tag { get; }
    /// <summary>
    /// 获取分类所属的文章链接列表。
    /// </summary>
    public IEnumerable<string> ArticleUris { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="DetailViewModel"/> 的新实例。
    /// </summary>
    /// <param name="tag">当前页要查看的标签。</param>
    /// <param name="articlesUris">分类所属的文章链接列表。</param>
    public DetailViewModel(string tag, IEnumerable<string> articlesUris)
    {
        Tag = tag;
        ArticleUris = articlesUris;
    }
}
