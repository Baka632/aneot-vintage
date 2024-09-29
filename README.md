[English](README-EN.md) | [原仓库](https://github.com/TCA-Arknights/aneot)

# 回归线 - 简易版

《明日方舟》同人文学期刊《回归线》，现在为旧式设备重新设计。

## 构建指引

- 克隆本仓库

- 确保 .NET 8 SDK 安装在您的电脑上

- 在 `src/AnEoT.Vintage` 文件夹中运行 `dotnet run` 命令

请尽情地阅读吧。

## 关于静态网站...

如果您想生成静态网站所需的文件，请在运行 `dotnet run` 命令时添加 ```-- static-only``` 参数。

默认情况下，静态网站所需的文件将在 `src/AnEoT.Vintage/StaticWebSite` 文件夹中生成。

若要改变生成位置，请使用下面的方法（按优先级排序）
- 使用 *命令行参数*：
```dotnet run --StaticWebSiteOutputPath "YOUR PATH" -- static-only```
- 设置名为 ```StaticWebSiteOutputPath``` 的 *环境变量* ：既可在 ```launchSettings.json``` 中设置，也可在命令行界面中配置。
- 在 *appsettings.json* 中添加名为 ```StaticWebSiteOutputPath``` 的键值对。
```
{
   ......
   "StaticWebSiteOutputPath": "YOUR PATH"
   ......
}
```

## 关于旧式设备的 WebP 图像支持...

大多数旧式设备不支持 WebP 图像，因此在这些设备中无法显示它们。

为了解决这个问题，我们启用了将 WebP 图像转换为 JPEG 图像的功能。

如果希望禁用此功能，请使用下面的方法（按优先级排序）

- 使用 *命令行参数*：
```dotnet run --ConvertWebP false```
- 设置名为 ```ConvertWebP``` 的*环境变量*： 既可在 ```launchSettings.json``` 中设置，也可在命令行界面中配置。
- 在 *appsettings.json* 中添加名为 ```ConvertWebP``` 的键值对。
```
{
   ......
   "ConvertWebP": false
   ......
}
```

此设置将影响静态网站生成。

## 此项目支持的浏览器

此项目最低支持的浏览器为 Internet Explorer 5.5（发布于 2000 年），比其更为老旧的浏览器不受支持。

## 关于我们

如果网页中有任何问题，或者您有任何意见，欢迎打开一个新的 Issue。

## 关于版权

请参阅[原仓库的 README](https://github.com/TCA-Arknights/aneot?tab=readme-ov-file#about-copyright)。

**以下内容翻译自原仓库，仅供参考，请以原仓库的 README 为准。**

---

我们仅授予您按原样保存和共享的权利。同时，您必须遵守以下要求：

**署名** —  您必须给出适当的署名，同时标明是否对原始作品作了修改。您可以用任何合理的方式来署名，但是不得以任何方式暗示许可人为您或您的使用背书。

**非商业性使用** — 您不得将材料用于商业目的。

**禁止演绎** — 如果您再混合、转换、或者基于该作品创作，您不可以分发修改作品。

**禁止用于模型训练** — 本刊所有内容不得用于人工智能生成模型训练，包括但不限于个人、科研、教学和商业化应用。

我们保留所有未授予您的权利，如果您违反上述要求，我们将撤回授予您的所有权利。

**Copyright © 2022-2024 All Rights Reserved**