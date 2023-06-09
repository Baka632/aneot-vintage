namespace AnEoT.Vintage.ViewModels.Home
{
    /// <summary>
    /// 为Home/Index页面提供信息的模型
    /// </summary>
    public class IndexViewModel
    {
        //TODO: 应改为显示最新文章

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
