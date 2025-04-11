// Modified from Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware
// Original Text: Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace AnEoT.Vintage.ReverseProxy;

public class TryHttpsUpgradeMiddleware
{
    private const int PortNotFound = -1;

    private readonly RequestDelegate _next;
    private readonly Lazy<int> _httpsPort;
    private readonly int _statusCode;

    private readonly IServerAddressesFeature? _serverAddressesFeature;
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public TryHttpsUpgradeMiddleware(RequestDelegate next, IOptions<HttpsRedirectionOptions> options, IConfiguration config, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(config);

        _next = next;
        _config = config;

        HttpsRedirectionOptions httpsRedirectionOptions = options.Value;
        _httpsPort = httpsRedirectionOptions.HttpsPort.HasValue
            ? new Lazy<int>(httpsRedirectionOptions.HttpsPort.Value)
            : new Lazy<int>(TryGetHttpsPort);
        _statusCode = httpsRedirectionOptions.RedirectStatusCode;
        _logger = loggerFactory.CreateLogger<HttpsRedirectionMiddleware>();
    }

    public TryHttpsUpgradeMiddleware(RequestDelegate next, IOptions<HttpsRedirectionOptions> options, IConfiguration config, ILoggerFactory loggerFactory,
        IServerAddressesFeature serverAddressesFeature)
        : this(next, options, config, loggerFactory)
    {
        _serverAddressesFeature = serverAddressesFeature ?? throw new ArgumentNullException(nameof(serverAddressesFeature));
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Request.IsHttps)
        {
            return _next(context);
        }

        if (context.Request.Headers.UpgradeInsecureRequests.Equals("1"))
        {
            int port = _httpsPort.Value;
            if (port == PortNotFound)
            {
                return _next(context);
            }

            HostString host = context.Request.Host;
            if (port != 443)
            {
                host = new HostString(host.Host, port);
            }
            else
            {
                host = new HostString(host.Host);
            }

            HttpRequest request = context.Request;
            string redirectUrl = UriHelper.BuildAbsolute(
                "https",
                host,
                request.PathBase,
                request.Path,
                request.QueryString);

            context.Response.StatusCode = _statusCode;
            context.Response.Headers.Location = redirectUrl;

            _logger.LogDebug("正在重定向到：{redirectUrl}", redirectUrl);
        }

        return Task.CompletedTask;
    }

    //  Returns PortNotFound (-1) if we were unable to determine the port.
    private int TryGetHttpsPort()
    {
        // The IServerAddressesFeature will not be ready until the middleware is Invoked,
        // Order for finding the HTTPS port:
        // 1. Set in the HttpsRedirectionOptions
        // 2. HTTPS_PORT environment variable
        // 3. IServerAddressesFeature
        // 4. Fail if not sets

        int? nullablePort = GetIntConfigValue("HTTPS_PORT") ?? GetIntConfigValue("ANCM_HTTPS_PORT");
        if (nullablePort.HasValue)
        {
            int port = nullablePort.Value;
            _logger.LogDebug("已从配置中读取到 HTTPS 端口：{port}", port);
            return port;
        }

        if (_serverAddressesFeature == null)
        {
            LogFailedToDeterminePort();
            return PortNotFound;
        }

        foreach (string address in _serverAddressesFeature.Addresses)
        {
            BindingAddress bindingAddress = BindingAddress.Parse(address);
            if (bindingAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                // If we find multiple different https ports specified, throw
                if (nullablePort.HasValue && nullablePort != bindingAddress.Port)
                {
                    throw new InvalidOperationException(
                        "Cannot determine the https port from IServerAddressesFeature, multiple values were found. " +
                        "Set the desired port explicitly on HttpsRedirectionOptions.HttpsPort.");
                }
                else
                {
                    nullablePort = bindingAddress.Port;
                }
            }
        }

        if (nullablePort.HasValue)
        {
            int port = nullablePort.Value;
            _logger.LogDebug("已从服务终结点发现 HTTPS 端口：{httpsPort}", port);
            return port;
        }

        LogFailedToDeterminePort();

        return PortNotFound;

        int? GetIntConfigValue(string name) =>
            int.TryParse(_config[name], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value) ? value : null;
    }

    private void LogFailedToDeterminePort()
    {
        _logger.LogWarning("无法确定 HTTPS 重定向所需要的端口");
    }
}