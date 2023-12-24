[中文](README.md) | [Original repository](https://github.com/TCA-Arknights/aneot)

# Another End of Terra (AnEoT) for Vintage Devices

A fanmade literary journal based on mobile game *Arknights*, redesigned for vintage devices.

Notice: This is a unofficial project.

## Build Instructions

- Clone the repository locally

- Make sure .NET 8 SDK is available on your computer

- Run `dotnet run` command in `src` folder

Enjoy reading~

## Notice for static web site

If you want to generate static files, add command line argument ```-- static-only```

By default, static web site will be generated in `src/StaticWebSite` folder

If you want to change it, use the following method to do that (ordered by the priority)
- Use *command line argument*:
```dotnet run --StaticWebSiteOutputPath "YOUR PATH" -- static-only```
- Set *environment variables* named ```StaticWebSiteOutputPath```: set it in ```launchSettings.json``` or in your command line interface

- Add ```StaticWebSiteOutputPath``` in *appsettings.json*
```
{
   ......
   "StaticWebSiteOutputPath": "YOUR PATH"
   ......
}
```

## About WebP image support for vintage devices...
Most vintage devices don't support WebP image, so WebP image can't be displayed on these devices.

To solve this problem, we can dynamically convert WebP image to JPEG image, but due to performance <!-- and copyright [I don't know whether AnEoT's offical members allow me to do this, even dynamically] -->  reason, this function is disabled by default.

To enable it, use the following method to do that (ordered by the priority)
- Use *command line argument*:
```dotnet run --ConvertWebP true```
- Set *environment variables* named ```ConvertWebP```: set it in ```launchSettings.json``` or in your command line interface

- Add ```ConvertWebP``` in *appsettings.json*
```
{
   ......
   "ConvertWebP": true
   ......
}
```

This configuration will affect static web site generation.

## About Us

If there are any errors in the website or you have any comments,

You can open a new issue.

## About copyright

See [README from the original repository](https://github.com/TCA-Arknights/aneot?tab=readme-ov-file#about-copyright)。