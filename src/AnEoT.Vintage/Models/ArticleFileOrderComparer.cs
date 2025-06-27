namespace AnEoT.Vintage.Models;

/// <summary>
/// 为表示文章文件的 <see cref="FileInfo"/> 提供比较方法，以用于排序。
/// </summary>
public sealed class ArticleFileOrderComparer : Comparer<FileInfo>
{
    /// <inheritdoc/>
    public override int Compare(FileInfo? x, FileInfo? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }
        else if (y is null)
        {
            return 1;
        }

        string markdownX = File.ReadAllText(x.FullName);
        string markdownY = File.ReadAllText(y.FullName);

        ArticleInfo articleInfoX = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdownX);
        ArticleInfo articleInfoY = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdownY);

        if (articleInfoX.Order > 0 && articleInfoY.Order > 0)
        {
            return Comparer<int>.Default.Compare(articleInfoX.Order, articleInfoY.Order);
        }
        else if (articleInfoX.Order > 0 && articleInfoY.Order < 0)
        {
            return -1;
        }
        else if (articleInfoX.Order < 0 && articleInfoY.Order > 0)
        {
            return 1;
        }
        else
        {
            return Comparer<int>.Default.Compare(-articleInfoX.Order, -articleInfoY.Order);
        }
    }
}