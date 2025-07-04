using System.ServiceModel.Syndication;
using System.Xml;
using System.Text;
using AnEoT.Vintage.Helpers.Custom;
using AnEoT.Vintage.Models;
using System.Globalization;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 协助生成订阅源的类。
/// </summary>
public partial class FeedGenerationHelper(
    CommonValuesHelper commonValues,
    CategoryAndTagHelper categoryAndTagHelper,
    IConfiguration configuration)
{
    private const string LxgwFontUri = "https://unpkg.com/lxgw-wenkai-screen-webfont@1.6.0/style.css";
    private readonly string[] CssLinks = ["css/site.css", "css/index.css", "css/rss-style.css", LxgwFontUri];

    /// <summary>
    /// 生成订阅源。
    /// </summary>
    /// <param name="includeAllArticles">确定订阅源是否包含全部文章信息的值。</param>
    /// <param name="generateDigest">指示生成模式是否为生成摘要的值。</param>
    /// <param name="rss20FileName">RSS 2.0 源的文件名。</param>
    /// <param name="atomFileName">Atom 源的文件名。</param>
    /// <returns>第一个元素为 RSS 文件路径，第二个元素为 Atom 源路径的二元组。</returns>
    public (string RssPath, string AtomPath) GenerateFeed(bool includeAllArticles = false, bool generateDigest = false, string rss20FileName = "rss.xml", string atomFileName = "atom.xml")
    {
        if (!bool.TryParse(configuration["FeedAddCssStyle"], out bool addCssStyle))
        {
            addCssStyle = true;
        }

        string webRootPath = commonValues.WebRootPath;
        Uri baseUri = commonValues.BaseUri;

        string? cssLinksString = null;
        if (addCssStyle)
        {
            StringBuilder sb = new(500);
            foreach (string link in CssLinks)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"""<link href="{new Uri(baseUri, link)}" rel ="stylesheet" type="text/css" />""");
            }
            cssLinksString = sb.ToString();
        }

        #region 第一步：生成订阅源信息
        string title = includeAllArticles switch
        {
            true when generateDigest => "回归线简易版 - 完整源摘要",
            true => "回归线简易版 - 完整源",
            false when generateDigest => "回归线简易版 - 摘要",
            false => "回归线简易版"
        };
        string id = includeAllArticles switch
        {
            true when generateDigest => "AnEoT-Vintage-Full-Digest",
            true => "AnEoT-Vintage-Full",
            false when generateDigest => "AnEoT-Vintage-Digest",
            false => "AnEoT-Vintage"
        };

        SyndicationFeed feed = new(
           title,
           "Another End of Terra",
           baseUri,
           id,
           DateTimeOffset.Now)
        {
            Copyright = new TextSyndicationContent($"泰拉创作者联合会保留所有权利 | Copyright © 2022-{DateTimeOffset.UtcNow.Year} TCA. All rights reserved."),
            Language = "zh-CN",
            Generator = "System.ServiceModel.Syndication.SyndicationFeed, used in AnEoT.Vintage",
            ImageUrl = new Uri(baseUri, "favicon.ico"),
        };

        IEnumerable<string> categories = categoryAndTagHelper.GetAllCategories();
        foreach (string item in categories)
        {
            feed.Categories.Add(new SyndicationCategory(item));
        }

        // 获取 posts 文件夹的信息
        DirectoryInfo postsDirectoryInfo = new(Path.Combine(webRootPath, "posts"));

        List<SyndicationItem> items = [];

        // 反向读取文件夹，以获取到最新的期刊
        List<DirectoryInfo> volDirInfos = [.. postsDirectoryInfo.EnumerateDirectories()];
        volDirInfos.Sort(new VolumeDirectoryOrderComparer());
        volDirInfos.Reverse();

        IEnumerable<DirectoryInfo> targetDirectories;
        if (includeAllArticles)
        {
            targetDirectories = volDirInfos;
        }
        else
        {
            // 我们只获取前两个文件夹的信息，避免订阅源过长
            targetDirectories = volDirInfos.Take(2);
        }

        foreach (DirectoryInfo volDirInfo in targetDirectories)
        {
            IOrderedEnumerable<FileInfo> articles = volDirInfo
                                .EnumerateFiles("*.md")
                                .Where(file => !file.Name.Contains("README.md"))
                                .Order(new ArticleFileOrderComparer());
            foreach (FileInfo article in articles)
            {
                string markdown = File.ReadAllText(article.FullName);
                ArticleInfo articleInfo = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown);

                Uri articleLink = new(baseUri, $"posts/{volDirInfo.Name}/{article.Name.Replace(".md", ".html")}");

                TextSyndicationContent content;
                CustomMarkdownParser parser = new(false, false, commonValues.ConvertWebP, new Uri(baseUri, $"posts/{volDirInfo.Name}").ToString(), true, true);

                if (generateDigest)
                {
                    string quote = MarkdownHelper.GetArticleQuote(markdown);
                    string bodyContent = parser.Parse(quote);

                    string htmlContentString;
                    if (string.IsNullOrWhiteSpace(bodyContent))
                    {
                        htmlContentString = $"""<a href="{articleLink}" target="_blank">请单击这里阅读全文......</a>""";
                    }
                    else
                    {
                        bodyContent = $"""
                            {bodyContent}
                            <br />
                            <a href="{articleLink}" target="_blank">请单击这里阅读全文......</a>
                            """;

                        if (addCssStyle)
                        {
                            htmlContentString = $"""
                        <head>
                            {cssLinksString}
                        </head>
                        <body>
                            {bodyContent}
                        </body>
                        """;
                        }
                        else
                        {
                            htmlContentString = bodyContent;
                        }
                    }

                    content = SyndicationContent.CreateHtmlContent(htmlContentString);
                }
                else
                {
                    string html = parser.Parse(markdown);

                    if (addCssStyle)
                    {
                        html = $"""
                      <head>
                          {cssLinksString}
                      </head>
                      <body>
                          {html}
                      </body>
                      """;
                    }

                    content = SyndicationContent.CreateHtmlContent(html);
                }

                bool hasDate = DateTimeOffset.TryParse(articleInfo.Date, out DateTimeOffset publishDate);
                DateTimeOffset timeNow = DateTimeOffset.Now;

                SyndicationItem item = new(
                    articleInfo.Title,
                    content,
                    articleLink,
                    articleLink.ToString(),
                    hasDate ? publishDate : timeNow);

                item.Authors.Add(new SyndicationPerson() { Name = articleInfo.Author });

                foreach (string category in articleInfo.Category ?? [])
                {
                    item.Categories.Add(new SyndicationCategory(category));
                }

                if (hasDate)
                {
                    item.PublishDate = publishDate;
                }
                else
                {
                    item.PublishDate = timeNow;
                }

                items.Add(item);
            }
        }

        feed.Items = items;
        #endregion

        #region 第二步：将订阅源信息序列化为 XML 文件
        XmlWriterSettings settings = new()
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true
        };


        string atomFilePath = Path.Combine(webRootPath, atomFileName);
        if (Path.Exists(atomFilePath))
        {
            File.Delete(atomFilePath);
        }

        XmlWriter atomWriter = XmlWriter.Create(atomFilePath, settings);
        Atom10FeedFormatter atomFormatter = new(feed);
        atomFormatter.WriteTo(atomWriter);
        atomWriter.Close();

        string rssFilePath = Path.Combine(webRootPath, rss20FileName);
        if (Path.Exists(rssFilePath))
        {
            File.Delete(rssFilePath);
        }

        XmlWriter rssWriter = XmlWriter.Create(rssFilePath, settings);
        Rss20FeedFormatter rssFormatter = new(feed);
        rssFormatter.WriteTo(rssWriter);
        rssWriter.Close();
        #endregion

        return (rssFilePath, atomFilePath);
    }
}

/// <summary>
/// 为 <see cref="FeedGenerationHelper"/> 提供扩展方法的类。
/// </summary>
public static partial class FeedGenerationHelperExtensions
{
    private const string LoggerName = "AnEoT.Vintage.FeedGenerator";

    /// <summary>
    /// 生成全部类型的订阅源。
    /// </summary>
    /// <param name="host">.NET 通用主机。</param>
    /// <returns>完成操作后的 <see cref="IHost"/>。</returns>
    public static IHost GenerateAllFeed(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger(LoggerName);
        IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

        logger.LogFeedGenerating();

        FeedGenerationHelper helper = host.Services.GetRequiredService<FeedGenerationHelper>();

        if (!bool.TryParse(configuration["FeedIncludeAllArticles"], out bool feedIncludeAllArticles))
        {
            feedIncludeAllArticles = true;
        }

        List<string> feedPaths = new(8);

        if (feedIncludeAllArticles)
        {
            helper.GenerateFeed(includeAllArticles: true, rss20FileName: "rss_full.xml", atomFileName: "atom_full.xml")
                .AddToList(feedPaths);
            helper.GenerateFeed(includeAllArticles: true, generateDigest: true, rss20FileName: "rss_full_digest.xml", atomFileName: "atom_full_digest.xml")
                .AddToList(feedPaths);
        }

        helper.GenerateFeed()
            .AddToList(feedPaths);
        helper.GenerateFeed(generateDigest: true, rss20FileName: "rss_digest.xml", atomFileName: "atom_digest.xml")
            .AddToList(feedPaths);

        logger.LogFeedGenerated(string.Join("\n\t", feedPaths));

        return host;
    }

    private static void AddToList(this ValueTuple<string, string> tuple, List<string> list)
    {
        list.Add(tuple.Item1);
        list.Add(tuple.Item2);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "正在生成订阅源......")]
    private static partial void LogFeedGenerating(this ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "已在以下路径中生成订阅源：\n\t{path}")]

    private static partial void LogFeedGenerated(this ILogger logger, string path);
}
