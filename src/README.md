# 项目结构说明

## AnEoT.Vintage 

主项目，负责呈现网页内容以及生成 RSS 源及静态网页相关文件。

类型为 ASP.NET Core MVC 应用程序。

## AnEoT.Vintage.Common

公用项目，负责共享各种模型类，如 ArticleInfo。

类型为 .NET 类库。

## AnEoT.Vintage.Tool

工具项目，负责对主项目进行各种操作，如文件预处理。

类型为控制台应用程序。

## AnEoT.Vintage.StaticServer

静态服务器项目，负责提供自定义的静态文件服务器（基于 Kestrel）。

类型为 ASP.NET Core Web 应用程序。

## AnEoT.Vintage.ReverseProxy

反向代理服务器项目，负责为后端服务器提供反向代理服务（基于 Yarp）。

类型为 ASP.NET Core Web 应用程序。

<!--
## AnEoT.Vintage.WebHook

Web API 项目，负责根据 GitHub WebHook 在服务器进行某些操作。

类型为 ASP.NET Core Web 应用程序。
-->