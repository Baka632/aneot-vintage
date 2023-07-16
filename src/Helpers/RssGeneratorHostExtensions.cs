using System.ServiceModel.Syndication;
using System.Xml;
using AnEoT.Vintage.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Westwind.AspNetCore.Markdown;
using Microsoft.AspNetCore.Http.Features;

namespace AnEoT.Vintage.Helpers
{
    /// <summary>
    /// 协助在程序中使用 RSS 生成功能的类
    /// </summary>
    public static class RssGeneratorHostExtensions
    {
        /// <summary>
        /// 生成 RSS 源
        /// </summary>
        /// <param name="host">当前程序的通用主机</param>
        public static void GenerateRssFeed(this IHost host)
        {
            //获取描述应用生命周期的IHostApplicationLifetime
            IHostApplicationLifetime requiredService = host.Services.GetRequiredService<IHostApplicationLifetime>();

            requiredService.ApplicationStarted.Register(() => StartGenerateRssFeed(host));
        }

        private static void StartGenerateRssFeed(IHost host)
        {
            IFeatureCollection hostFeatures = host.Services.GetRequiredService<IServer>().Features;
            IServerAddressesFeature serverAddresses = hostFeatures.Get<IServerAddressesFeature>()
                ?? throw new InvalidOperationException("功能IServerAddressesFeature现在不可用");
            ICollection<string> hostUrls = serverAddresses.Addresses;

            string baseUri =
                (hostUrls.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttps)) ?? hostUrls.FirstOrDefault(x => x.StartsWith(Uri.UriSchemeHttp)))
                ?? throw new InvalidOperationException("现在无法获取到基Uri");

            //获取IWebHostEnvironment来确定放置Feed的文件夹
            IWebHostEnvironment env = host.Services.GetService<IWebHostEnvironment>()
                ?? throw new ArgumentException("IWebHostEnvironment现在不可用");

            SyndicationFeed feed = new(
                "回归线",
                "Another End of Terra",
                new Uri("https://aneot.terrach.net"),
                "AnEoT",
                DateTime.Now)
            {
                Copyright = new TextSyndicationContent("泰拉创作者联合会保留所有权利 | Copyright © 2022-2023 TCA. All rights reserved."),
                Language = "zh-CN",
                Generator = "System.ServiceModel.Syndication.SyndicationFeed, used in AnEoT.Vintage",
                ImageUrl = new Uri($"{baseUri}/images/logo.jpg"),
            };

            SyndicationCategory[] categories = new SyndicationCategory[]
            {
                new("干员秘闻"),
                new("此地之外"),
                new("罗德岛日志"),
                new("午间逸话"),
                new("画中秘境"),
                new("特别企划"),
                new("创作者访谈"),
            };

            foreach (var item in categories)
            {
                feed.Categories.Add(item);
            }

            //获取posts文件夹的信息
            DirectoryInfo postsDirectoryInfo = new(Path.Combine(env.WebRootPath, "posts"));

            List<SyndicationItem> items = new();

            //反向读取文件夹，以获取到最新的期刊；我们只生成前两个期刊的信息，避免RSS源过长
            foreach (DirectoryInfo volDirInfo in postsDirectoryInfo.EnumerateDirectories().Reverse().Take(2))
            {
                foreach (FileInfo article in volDirInfo.EnumerateFiles("*.md"))
                {
                    //去掉README.md，因为RSS源不应该包含这个东西
                    if (article.Name.Contains("README.md"))
                    {
                        continue;
                    }

                    string markdown = File.ReadAllText(article.FullName);
                    FrontMatter frontMatter = MarkdownHelper.GetFrontMatter<FrontMatter>(markdown);
                    string articleLink = $"{baseUri}/posts/{volDirInfo.Name}/{article.Name.Replace(".md", ".html")}";
                    
                    string html = Markdown.Parse(markdown);
                    TextSyndicationContent content = SyndicationContent.CreateHtmlContent(html);

                    SyndicationItem item = new(
                        frontMatter.Title,
                        content,
                        new Uri(articleLink, UriKind.RelativeOrAbsolute),
                        articleLink,
                        DateTime.Now);

                    item.Authors.Add(new SyndicationPerson() { Name = frontMatter.Author });

                    foreach (var category in frontMatter.Category ?? Array.Empty<string>())
                    {
                        item.Categories.Add(new SyndicationCategory(category));
                    }

                    if (DateTimeOffset.TryParse(frontMatter.Date, out DateTimeOffset publishDate))
                    {
                        item.PublishDate = publishDate;
                    }

                    items.Add(item);
                }
            }

            feed.Items = items;

            XmlWriter atomWriter = XmlWriter.Create(Path.Combine(env.WebRootPath, "atom.xml"));
            Atom10FeedFormatter atomFormatter = new(feed);
            atomFormatter.WriteTo(atomWriter);
            atomWriter.Close();

            XmlWriter rssWriter = XmlWriter.Create(Path.Combine(env.WebRootPath, "rss.xml"));
            Rss20FeedFormatter rssFormatter = new(feed);
            rssFormatter.WriteTo(rssWriter);
            rssWriter.Close();
        }
    }
}
