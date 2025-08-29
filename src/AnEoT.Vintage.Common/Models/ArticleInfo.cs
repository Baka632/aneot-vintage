using System.Linq;

namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示文章信息的结构。
/// </summary>
public struct ArticleInfo : IEquatable<ArticleInfo>, IComparable<ArticleInfo>
{
    /// <summary>
    /// 构造一个已按默认值初始化的 <see cref="ArticleInfo"/> 的新实例。
    /// </summary>
    public ArticleInfo()
    {
    }

    /// <summary>
    /// 文档标题。
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 文档短标题。
    /// </summary>
    public string ShortTitle { get; set; } = string.Empty;
    /// <summary>
    /// 文档类型图标。
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    /// <summary>
    /// 指示该项是否为文章的值。
    /// </summary>
    public bool Article { get; set; } = true;
    /// <summary>
    /// 文章作者。
    /// </summary>
    public string? Author { get; set; }
    /// <summary>
    /// 文档创建日期的字符串。
    /// </summary>
    public string? Date { get; set; }
    /// <summary>
    /// 文档分类。
    /// </summary>
    public IEnumerable<string>? Category { get; set; }
    /// <summary>
    /// 文档标签。
    /// </summary>
    public IEnumerable<string>? Tag { get; set; }
    /// <summary>
    /// 文档在本期期刊的顺序。
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// 
    /// </summary>
    //我也不知道这有什么用
    public IDictionary<string, int>? Dir { get; set; }

    /// <summary>
    /// <!-- 摸鱼~摸鱼~我就是不写文档注释~咕噜咕噜~和缪缪一起摸鱼~ -->
    /// <!-- 咕噜咕噜~现在还要和尤里卡一起~继续大地旅途~ -->
    /// </summary>
    public bool Star { get; set; }

    /// <summary>
    /// 指示是否索引此文章的值。
    /// </summary>
    public bool Index { get; set; }

    /// <summary>
    /// 指示是否在导航栏中呈现此文章的值。
    /// </summary>
    public bool Narbar { get; set; }

    /// <summary>
    /// 指示是否在侧边栏中呈现此文章的值。
    /// </summary>
    public bool Sidebar { get; set; }

    /// <summary>
    /// 页面描述。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 比较两个 <see cref="ArticleInfo"/> 实例，以确定排序顺序。
    /// </summary>
    /// <param name="other">另一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示比较结果的 <see langword="int"/>。有关其含义，请查阅 <see cref="IComparable{T}.CompareTo(T)"/> 的文档。</returns>
    public readonly int CompareTo(ArticleInfo other)
    {
        if (this.Order > 0 && other.Order > 0)
        {
            return Comparer<int>.Default.Compare(this.Order, other.Order);
        }
        else if (this.Order > 0 && other.Order < 0)
        {
            return -1;
        }
        else if (this.Order < 0 && other.Order > 0)
        {
            return 1;
        }
        else
        {
            return Comparer<int>.Default.Compare(this.Order, other.Order);
        }
    }

    /// <inheritdoc/>
    public readonly override bool Equals(object? obj)
    {
        return obj is ArticleInfo info && Equals(info);
    }

    /// <inheritdoc/>
    public readonly bool Equals(ArticleInfo other)
    {
        bool isCategoryEqual = Category is not null && other.Category is not null
            ? Category.SequenceEqual(other.Category)
            : object.ReferenceEquals(Category, other.Category);
        
        bool isTagEqual = Tag is not null && other.Tag is not null
            ? Tag.SequenceEqual(other.Tag)
            : object.ReferenceEquals(Tag, other.Tag);
        
        bool isDirEqual = Dir is not null && other.Dir is not null
            ? Dir.SequenceEqual(other.Dir)
            : object.ReferenceEquals(Dir, other.Dir);

        return Title == other.Title &&
               ShortTitle == other.ShortTitle &&
               Icon == other.Icon &&
               Article == other.Article &&
               Author == other.Author &&
               Date == other.Date &&
               isCategoryEqual &&
               isTagEqual &&
               Order == other.Order &&
               isDirEqual &&
               Star == other.Star &&
               Index == other.Index &&
               Description == other.Description;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Title);
        hash.Add(ShortTitle);
        hash.Add(Icon);
        hash.Add(Article);
        hash.Add(Author);
        hash.Add(Date);
        hash.Add(Category);
        hash.Add(Tag);
        hash.Add(Order);
        hash.Add(Dir);
        hash.Add(Star);
        hash.Add(Index);
        hash.Add(Description);
        return hash.ToHashCode();
    }

    /// <summary>
    /// 确定两个 <see cref="ArticleInfo"/> 实例是否相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示二者是否相等的值。</returns>
    public static bool operator ==(ArticleInfo left, ArticleInfo right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 确定两个 <see cref="ArticleInfo"/> 实例是否不同。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示二者是否不同的值。</returns>
    public static bool operator !=(ArticleInfo left, ArticleInfo right)
    {
        return !(left == right);
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfo"/> 实例的顺序是否在右边的 <see cref="ArticleInfo"/> 前面。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例前面的值。</returns>
    public static bool operator <(ArticleInfo left, ArticleInfo right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfo"/> 实例的顺序是否在右边的 <see cref="ArticleInfo"/> 前面，抑或排序顺序相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例前面，或者两个实例排序相等的值。</returns>
    public static bool operator <=(ArticleInfo left, ArticleInfo right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfo"/> 实例的顺序是否在右边的 <see cref="ArticleInfo"/> 后面。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例后面的值。</returns>
    public static bool operator >(ArticleInfo left, ArticleInfo right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfo"/> 实例的顺序是否在右边的 <see cref="ArticleInfo"/> 后面，抑或排序顺序相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例后面，或者两个实例排序相等的值。</returns>
    public static bool operator >=(ArticleInfo left, ArticleInfo right)
    {
        return left.CompareTo(right) >= 0;
    }
}
