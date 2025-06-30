namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为获取网站基 Uri 提供帮助的类。
/// </summary>
public class BaseUriHelper
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
    /// 构造 <see cref="BaseUriHelper"/> 的新实例。
    /// </summary>
    /// <param name="configuration">提供配置信息的 <see cref="IConfiguration"/>。</param>
    /// <exception cref="InvalidOperationException">无法获取到基 Uri。</exception>
    public BaseUriHelper(IConfiguration configuration)
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
    }
}
