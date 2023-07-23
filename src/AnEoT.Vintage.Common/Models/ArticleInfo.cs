using System.Linq;

namespace AnEoT.Vintage.Common.Models;

/// <summary>
/// 表示文章信息的结构
/// </summary>
public struct ArticleInfo : IEquatable<ArticleInfo>
{
    /// <summary>
    /// 构造一个已按默认值初始化的<see cref="ArticleInfo"/>的新实例
    /// </summary>
    public ArticleInfo()
    {
    }

    /// <summary>
    /// 文档标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 文档短标题
    /// </summary>
    public string ShortTitle { get; set; } = string.Empty;
    /// <summary>
    /// 文档类型图标
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    /// <summary>
    /// 指示该项是否为文章的值
    /// </summary>
    public bool Article { get; set; } = true;
    /// <summary>
    /// 文章作者
    /// </summary>
    public string? Author { get; set; }
    /// <summary>
    /// 文档创建日期的字符串
    /// </summary>
    public string? Date { get; set; }
    /// <summary>
    /// 文档类别
    /// </summary>
    public IEnumerable<string>? Category { get; set; }
    /// <summary>
    /// 文档标签
    /// </summary>
    public IEnumerable<string>? Tag { get; set; }
    /// <summary>
    /// 文档在本期期刊的顺序
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// 
    /// </summary>
    //我也不知道这有什么用
    public IDictionary<string, int>? Dir { get; set; }

    /// <summary>
    /// <!--???-->
    /// </summary>
    public bool Star { get; set; }

    /// <summary>
    /// <!--???-->
    /// </summary>
    public bool Index { get; set; }

    /// <summary>
    /// 页面描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public readonly override bool Equals(object? obj)
    {
        return obj is ArticleInfo info && Equals(info);
    }

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

    public static bool operator ==(ArticleInfo left, ArticleInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ArticleInfo left, ArticleInfo right)
    {
        return !(left == right);
    }
}
