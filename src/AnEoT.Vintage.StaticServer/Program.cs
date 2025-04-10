WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseKestrelHttpsConfiguration();

WebApplication app = builder.Build();

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

app.Run();