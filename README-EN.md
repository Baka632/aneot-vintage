[中文](README.md) | [Original repository](https://github.com/TCA-Arknights/aneot)

# Another End of Terra (AnEoT) for Vintage Devices

A fanmade literary journal based on mobile game *Arknights*, redesigned for vintage devices now.

## Build Instructions

- Clone the repository locally

- Make sure .NET 10 SDK is available on your computer

- Run `dotnet run` command in `src/AnEoT.Vintage` folder

Enjoy reading~

## Notice for static web site

If you want to generate static files, add command line argument ```-- static-only```

By default, static web site will be generated in `src/AnEoT.Vintage/StaticWebSite` folder

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

To solve this problem, we enabled the feature that converts WebP image to JPEG image.

If you want to disable it, use the following method to do that (ordered by the priority)
- Use *command line argument*:
```dotnet run --ConvertWebP false```
- Set *environment variables* named ```ConvertWebP```: set it in ```launchSettings.json``` or in your command line interface.
- Add ```ConvertWebP``` in *appsettings.json*.
```
{
   ......
   "ConvertWebP": false
   ......
}
```

This configuration will affect static web site generation.

## Browsers that supported by this project

The minimum supported browser version for this project is Internet Explorer 5.5 (Released in 2000), and older browsers than it are not supported.

## About Us

If there are any errors in the website or you have any suggestions, you can open a new issue.

## About copyright

See [README from the original repository](https://github.com/TCA-Arknights/aneot?tab=readme-ov-file#about-copyright)。