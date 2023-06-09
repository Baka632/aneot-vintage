namespace AnEoT.Vintage.ViewModels.Posts
{
    /// <summary>
    /// 为Posts/Index页面提供信息的模型
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// aneot/posts内文件夹的信息
        /// </summary>
        public IEnumerable<DirectoryInfo> Directories { get; set; }

        /// <summary>
        /// 构造<see cref="IndexViewModel"/>的新实例
        /// </summary>
        public IndexViewModel(IEnumerable<DirectoryInfo> directories)
        {
            Directories = directories;
        }
    }
}
