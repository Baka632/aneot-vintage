using System.ServiceModel.Syndication;
using System.Xml;
using System.Text;
using AnEoT.Vintage.Helpers.Custom;

namespace AnEoT.Vintage.Helpers
{
    /// <summary>
    /// 协助在程序中使用 RSS 生成功能的类
    /// </summary>
    public static class RssGenerationHelper
    {
        /// <summary>
        /// 生成 RSS 源
        /// </summary>
        /// <param name="rssBaseUri">RSS 源的基Uri</param>
        /// <param name="webRootPath">“wwwroot”文件夹所在路径</param>
        public static void GenerateRssFeed(string rssBaseUri, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(rssBaseUri))
            {
                throw new ArgumentException($"“{nameof(rssBaseUri)}”不能为 null 或空白。", nameof(rssBaseUri));
            }

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                throw new ArgumentException($"“{nameof(webRootPath)}”不能为 null 或空白。", nameof(webRootPath));
            }

            Console.WriteLine("正在生成 RSS 源...");

            #region 第一步：生成RSS源信息
            SyndicationFeed feed = new(
               "回归线简易版",
               "Another End of Terra",
               new Uri(rssBaseUri),
               "AnEoT",
               DateTime.Now)
            {
                Copyright = new TextSyndicationContent("泰拉创作者联合会保留所有权利 | Copyright © 2022-2023 TCA. All rights reserved."),
                Language = "zh-CN",
                Generator = "System.ServiceModel.Syndication.SyndicationFeed, used in AnEoT.Vintage",
                ImageUrl = new Uri($"{rssBaseUri}/images/logo.jpg"),
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
            DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));

            List<SyndicationItem> items = new();

            //反向读取文件夹，以获取到最新的期刊；我们只生成前两个期刊的信息，避免RSS源过长
            IEnumerable<DirectoryInfo> volDirInfos = postsDirectoryInfo.EnumerateDirectories().Reverse().Take(2);
            foreach (DirectoryInfo volDirInfo in volDirInfos)
            {
                IOrderedEnumerable<FileInfo> articles = volDirInfo
                                    .EnumerateFiles("*.md")
                                    .Where(file => !file.Name.Contains("README.md"))
                                    .Order(new ArticleOrderComparer());
                foreach (FileInfo article in articles)
                {
                    string markdown = File.ReadAllText(article.FullName);
                    ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);
                    string articleLink = $"{rssBaseUri}/posts/{volDirInfo.Name}/{article.Name.Replace(".md", ".html")}";

                    CustomMarkdownParser parser = new(false, false, true, $"{rssBaseUri}/posts/{volDirInfo.Name}");
                    string html = parser.Parse(markdown);
                    TextSyndicationContent content = SyndicationContent.CreateHtmlContent(html);

                    SyndicationItem item = new(
                        articleInfo.Title,
                        content,
                        new Uri(articleLink, UriKind.Absolute),
                        articleLink,
                        DateTime.Now);

                    item.Authors.Add(new SyndicationPerson() { Name = articleInfo.Author });

                    foreach (var category in articleInfo.Category ?? Array.Empty<string>())
                    {
                        item.Categories.Add(new SyndicationCategory(category));
                    }

                    if (DateTimeOffset.TryParse(articleInfo.Date, out DateTimeOffset publishDate))
                    {
                        item.PublishDate = publishDate;
                    }

                    items.Add(item);
                }
            }

            feed.Items = items;
            #endregion

            #region 第二步：将RSS源信息序列化为XML文件
            XmlWriterSettings settings = new()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            };


            string atomFilePath = Path.Combine(webRootPath, "atom.xml");
            if (Path.Exists(atomFilePath))
            {
                File.Delete(atomFilePath);
            }

            XmlWriter atomWriter = XmlWriter.Create(atomFilePath, settings);
            Atom10FeedFormatter atomFormatter = new(feed);
            atomFormatter.WriteTo(atomWriter);
            atomWriter.Close();

            string rssFilePath = Path.Combine(webRootPath, "rss.xml");
            if (Path.Exists(rssFilePath))
            {
                File.Delete(rssFilePath);
            }

            XmlWriter rssWriter = XmlWriter.Create(rssFilePath, settings);
            Rss20FeedFormatter rssFormatter = new(feed);
            rssFormatter.WriteTo(rssWriter);
            rssWriter.Close();
            #endregion

            Console.WriteLine("RSS 源生成完成！");
        }
    }

    /// <summary>
    /// 为表示文章的<see cref="FileInfo"/>提供比较方法，以用于排序
    /// </summary>
    file sealed class ArticleOrderComparer : Comparer<FileInfo>
    {
        /// <inheritdoc/>
        public override int Compare(FileInfo? x, FileInfo? y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }

            string markdownX = File.ReadAllText(x.FullName);
            string markdownY = File.ReadAllText(y.FullName);

            ArticleInfo articleInfoX = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdownX);
            ArticleInfo articleInfoY = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdownY);

            if (articleInfoX.Order > 0 && articleInfoY.Order > 0)
            {
                return Comparer<int>.Default.Compare(articleInfoX.Order, articleInfoY.Order);
            }
            else if (articleInfoX.Order > 0 && articleInfoY.Order < 0)
            {
                return -1;
            }
            else if (articleInfoX.Order < 0 && articleInfoY.Order > 0)
            {
                return 1;
            }
            else
            {
                return Comparer<int>.Default.Compare(-articleInfoX.Order, -articleInfoY.Order);
            }
        }
    }
}
