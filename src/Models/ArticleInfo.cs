namespace AnEoT.Vintage.Models
{
    /// <summary>
    /// 表示文章信息的结构
    /// </summary>
    public struct ArticleInfo
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
        /// 文档创建日期
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
    }
}
