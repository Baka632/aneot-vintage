using AspNetStatic;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为获取应用程序常用配置提供帮助的类。
/// </summary>
public class CommonValuesHelper
{
    /// <summary>
    /// 网站的基 Uri 字符串。
    /// </summary>
    public string BaseUriString { get; }

    /// <summary>
    /// 网站的基 <see cref="Uri"/>。
    /// </summary>
    public Uri BaseUri { get; }

    /// <summary>
    /// 确定是否转换 WebP 图像的值。
    /// </summary>
    public bool ConvertWebP { get; }

    /// <summary>
    /// “wwwroot”文件夹路径。
    /// </summary>
    public string WebRootPath { get; set; }

    /// <summary>
    /// 构造 <see cref="CommonValuesHelper"/> 的新实例。
    /// </summary>
    /// <param name="configuration">提供配置信息的 <see cref="IConfiguration"/>。</param>
    /// <param name="environment">提供环境信息的 <see cref="IWebHostEnvironment"/>。</param>
    /// <exception cref="InvalidOperationException">无法获取到基 Uri。</exception>
    public CommonValuesHelper(IConfiguration configuration, IWebHostEnvironment environment)
    {
        string[]? hostUrls = configuration["urls"]?.Split(';');
        string? rssBaseUriInConfig = configuration["BaseUri"];

        if (string.IsNullOrWhiteSpace(rssBaseUriInConfig))
        {
            BaseUriString = hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
                    ?? hostUrls?.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException("无法获取到基 Uri。");
        }
        else
        {
            BaseUriString = rssBaseUriInConfig;
        }

        BaseUri = new Uri(BaseUriString, UriKind.Absolute);


        WebRootPath = environment.WebRootPath;


        if (!bool.TryParse(configuration["ConvertWebP"], out bool convertWebP))
        {
            convertWebP = true;
        }
        ConvertWebP = convertWebP;

#if DEBUG
        // 在调试模式及未启用静态网页生成的情况下，禁用 WebP 转换
        // 因为这种情况下，一般是正在进行调试，因此不应该启用这个功能，否则图像显示会出现问题
        if (!Environment.GetCommandLineArgs().HasExitWhenDoneArg())
        {
            ConvertWebP = false;
        }
#endif
    }

    /// <summary>
    /// 更改 WebP 图像路径，以将图像路径指向转换后的图像。
    /// </summary>
    /// <remarks>
    /// 如果没有启用转换 WebP 图像功能，则返回未修改过的路径。
    /// </remarks>
    /// <param name="path">WebP 图像路径。</param>
    /// <param name="newExtension">转换后图像的扩展名，默认为 JPG。</param>
    /// <returns>更改后的图像路径。</returns>
    public string ChangeWebPImagePath(string path, string newExtension = ".jpg")
    {
        return ConvertWebP
            ? Path.ChangeExtension(path, newExtension)
            : path;
    }
}
