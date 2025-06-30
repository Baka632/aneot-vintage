using Microsoft.AspNetCore.Http.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("aneot-vintage-reverse-proxy-config.json", false);

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddLettuceEncrypt();
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365 * 2);
    });
    builder.WebHost.ConfigureKestrel(kestrelOptions =>
    {
        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.UseLettuceEncrypt(kestrelOptions.ApplicationServices);
        });
    });
}

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    if (!context.Request.IsHttps && context.Request.Headers.UpgradeInsecureRequests.Equals("1"))
    {
        HttpRequest request = context.Request;
        string redirectUrl = UriHelper.BuildAbsolute(
            "https",
            request.Host,
            request.PathBase,
            request.Path,
            request.QueryString);

        context.Response.StatusCode = StatusCodes.Status308PermanentRedirect;
        context.Response.Headers.Location = redirectUrl;
        context.Response.Headers.Vary = "Upgrade-Insecure-Requests";
        return;
    }
    else
    {
        await next(context);
    }
});

app.UseStatusCodePages();

app.MapReverseProxy();

app.Run();