using AnEoT.Vintage.ReverseProxy;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromMinutes(10);
});

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

        context.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
        context.Response.Headers.Location = redirectUrl;
        context.Response.Headers.Vary = "Upgrade-Insecure-Requests";
        return;
    }
    else
    {
        await next(context);
    }
});

app.MapReverseProxy();

app.Run();