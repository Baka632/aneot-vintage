using System.Linq;

namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示文章信息的结构。
/// </summary>
/// <param name="Title">文章标题。</param>
/// <param name="ShortTitle">文章短标题。</param>
/// <param name="Icon">文章类型图标。</param>
/// <param name="Article">指示该项是否为文章的值。</param>
/// <param name="Author">文章作者。</param>
/// <param name="Date">文章创建日期的字符串。</param>
/// <param name="Category">文章分类。</param>
/// <param name="Tag">文章标签。</param>
/// <param name="Order">文章在本期期刊的顺序。</param>
/// <param name="Dir"><!-- 我也不知道这有什么用 --></param>
/// <param name="Star"><!-- 摸鱼~摸鱼~我就是不写文档注释~咕噜咕噜~和缪缪一起摸鱼~ 咕噜咕噜~现在还要和尤里卡一起~继续大地旅途~ --></param>
/// <param name="Index">指示是否索引此文章的值。</param>
/// <param name="Narbar">指示是否在导航栏中呈现此文章的值。</param>
/// <param name="Sidebar">指示是否在侧边栏中呈现此文章的值。</param>
/// <param name="Description">文章描述。</param>
/// <param name="Sticky">指示在首页固定文章的顺序。</param>
/// <param name="ArticleQuote">
/// <para>文章引言。</para>
/// <para>此属性需要手动赋值，从 Front Matter 中无法获得。</para>
/// </param>
public record struct ArticleInfo(
    string Title, string ShortTitle, string Icon, bool Article, string? Author, string? Date,
    IEnumerable<string>? Category, IEnumerable<string>? Tag, int Order, IDictionary<string, int>? Dir, bool Star,
    bool Index, bool Narbar, bool Sidebar, string Description, int Sticky, string ArticleQuote) : IComparable<ArticleInfo>, IEquatable<ArticleInfo>
{
    /// <summary>
    /// 构造一个已按默认值初始化的 <see cref="ArticleInfo"/> 的新实例。
    /// </summary>
    public ArticleInfo() : this(string.Empty, string.Empty, string.Empty, true, default, default, default, default, default, default, default, default, default, default, string.Empty, default, string.Empty)
    {
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

    /// <inheritdoc />
    public readonly bool Equals(ArticleInfo info)
    {
        bool isCategoryEqual = Category is not null && info.Category is not null
            ? Category.SequenceEqual(info.Category)
            : object.ReferenceEquals(Category, info.Category);

        bool isTagEqual = Tag is not null && info.Tag is not null
            ? Tag.SequenceEqual(info.Tag)
            : object.ReferenceEquals(Tag, info.Tag);

        bool isDirEqual = Dir is not null && info.Dir is not null
            ? Dir.SequenceEqual(info.Dir)
            : object.ReferenceEquals(Dir, info.Dir);

        return Title == info.Title &&
               ShortTitle == info.ShortTitle &&
               Icon == info.Icon &&
               Article == info.Article &&
               Author == info.Author &&
               Date == info.Date &&
               isCategoryEqual &&
               isTagEqual &&
               Order == info.Order &&
               isDirEqual &&
               Star == info.Star &&
               Index == info.Index &&
               Narbar == info.Narbar &&
               Sidebar == info.Sidebar &&
               Description == info.Description &&
               Sticky == info.Sticky &&
               ArticleQuote == info.ArticleQuote;
    }

    /// <inheritdoc />
    public readonly override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Title);
        hash.Add(ShortTitle);
        hash.Add(Icon);
        hash.Add(Article);
        hash.Add(Author);
        hash.Add(Date);
        AddEnumerableToHash(ref hash, Category);
        AddEnumerableToHash(ref hash, Tag);
        hash.Add(Order);
        AddEnumerableToHash(ref hash, Dir);
        hash.Add(Star);
        hash.Add(Index);
        hash.Add(Narbar);
        hash.Add(Sidebar);
        hash.Add(Description);
        hash.Add(Sticky);
        hash.Add(ArticleQuote);

        return hash.ToHashCode();

        static void AddEnumerableToHash<T>(ref HashCode hash, IEnumerable<T>? enumerable)
        {
            if (enumerable is null)
            {
                return;
            }

            foreach (T item in enumerable)
            {
                hash.Add(item);
            }
        }
    }
}


/// <summary>
/// 表示文章信息的结构。
/// </summary>
[Obsolete("Use ArticleInfo instead.")]
public struct ArticleInfoOld : IEquatable<ArticleInfoOld>, IComparable<ArticleInfoOld>
{
    /// <summary>
    /// 
    /// </summary>
    public ArticleInfoOld()
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
    /// 指示在首页固定文章的顺序。
    /// </summary>
    public int Sticky { get; set; }

    /// <summary>
    /// 比较两个 <see cref="ArticleInfo"/> 实例，以确定排序顺序。
    /// </summary>
    /// <param name="other">另一个 <see cref="ArticleInfo"/> 实例。</param>
    /// <returns>指示比较结果的 <see langword="int"/>。有关其含义，请查阅 <see cref="IComparable{T}.CompareTo(T)"/> 的文档。</returns>
    public readonly int CompareTo(ArticleInfoOld other)
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
        return obj is ArticleInfoOld info && Equals(info);
    }

    /// <inheritdoc/>
    public readonly bool Equals(ArticleInfoOld other)
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
               Description == other.Description &&
               Sticky == other.Sticky;
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
        hash.Add(Sticky);
        return hash.ToHashCode();
    }

    /// <summary>
    /// 确定两个 <see cref="ArticleInfoOld"/> 实例是否相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示二者是否相等的值。</returns>
    public static bool operator ==(ArticleInfoOld left, ArticleInfoOld right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// 确定两个 <see cref="ArticleInfoOld"/> 实例是否不同。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示二者是否不同的值。</returns>
    public static bool operator !=(ArticleInfoOld left, ArticleInfoOld right)
    {
        return !(left == right);
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfoOld"/> 实例的顺序是否在右边的 <see cref="ArticleInfoOld"/> 前面。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例前面的值。</returns>
    public static bool operator <(ArticleInfoOld left, ArticleInfoOld right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfoOld"/> 实例的顺序是否在右边的 <see cref="ArticleInfoOld"/> 前面，抑或排序顺序相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例前面，或者两个实例排序相等的值。</returns>
    public static bool operator <=(ArticleInfoOld left, ArticleInfoOld right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfoOld"/> 实例的顺序是否在右边的 <see cref="ArticleInfoOld"/> 后面。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例后面的值。</returns>
    public static bool operator >(ArticleInfoOld left, ArticleInfoOld right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// 确定左边的 <see cref="ArticleInfoOld"/> 实例的顺序是否在右边的 <see cref="ArticleInfoOld"/> 后面，抑或排序顺序相等。
    /// </summary>
    /// <param name="left">第一个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <param name="right">第二个 <see cref="ArticleInfoOld"/> 实例。</param>
    /// <returns>指示左侧实例顺序是否在右侧实例后面，或者两个实例排序相等的值。</returns>
    public static bool operator >=(ArticleInfoOld left, ArticleInfoOld right)
    {
        return left.CompareTo(right) >= 0;
    }
}