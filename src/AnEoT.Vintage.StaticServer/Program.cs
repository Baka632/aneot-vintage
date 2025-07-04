using Microsoft.AspNetCore.ResponseCompression;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseKestrelHttpsConfiguration();
builder.Services.AddResponseCompression(options =>
{
    // HACK: 如果此服务器需要承载需要身份验证的内容，那么需要重新评估响应压缩的安全性。
    options.EnableForHttps = true;
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["image/svg+xml"]);
});

WebApplication app = builder.Build();

// 如果此服务器需要承载需要身份验证的内容
// 那么需要重新评估响应压缩的安全性
// 当然，如果 MapStaticAssets 可用了，我们就可以不用这个了
app.UseResponseCompression();

app.Use(async (context, next) =>
{
    context.Response.Headers.Server = "AnEoT Vintage Server";
    context.Response.Headers.XPoweredBy = "Terra Creator Association";
    await next();
});

app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = ["index.html"]
});
app.UseStaticFiles();
// 需要等待：https://github.com/dotnet/aspnetcore/issues/59399
// app.MapStaticAssets();

app.Run();