using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;

namespace AnEoT.Vintage.Common.Helpers;

/// <summary>
/// 为 Markdown 处理提供通用操作
/// </summary>
public static class MarkdownHelper
{
    private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UseAdvancedExtensions()
            .UseListExtras()
            .UseEmojiAndSmiley(true)
            .UseYamlFrontMatter()
            .Build();

    /// <summary>
    /// 获取由Markdown中Front Matter转换而来的模型
    /// </summary>
    /// <param name="markdown">Markdown文件内容</param>
    /// <typeparam name="T">模型类型</typeparam>
    /// <returns>转换得到的模型</returns>
    [RequiresDynamicCode("此方法调用了不支持 IL 裁剪的 AnEoT.Vintage.Tool.Helpers.YamlHelper.ReadYaml<T>(String)")]
    public static T GetFromFrontMatter<T>(string markdown)
    {
        MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
        YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        if (yamlBlock is not null)
        {
            string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
            T model = YamlHelper.ReadYaml<T>(yaml);

            return model;
        }
        else
        {
            throw new ArgumentException("无法通过指定的参数解析出模型，Markdown可能没有Front Matter信息");
        }
    }

    /// <summary>
    /// 尝试获取由 Markdown 中 Front Matter 转换而来的模型
    /// </summary>
    /// <param name="markdown">Markdown 文件内容</param>
    /// <param name="result">转换得到的模型</param>
    /// <typeparam name="T">模型类型</typeparam>
    /// <returns>指示操作是否成功的值</returns>
    [RequiresDynamicCode("此方法调用了不支持 IL 裁剪的 AnEoT.Vintage.Tool.Helpers.YamlHelper.TryReadYaml<T>(String, out T)")]
    public static bool TryGetFromFrontMatter<T>(string markdown, [MaybeNullWhen(false)] out T result)
    {
        MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
        YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        if (yamlBlock is not null)
        {
            string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
            if (YamlHelper.TryReadYaml(yaml, out T? model) && model is not null)
            {
                result = model;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// 获取 Markdown 的文章引言
    /// </summary>
    /// <param name="markdown">Markdown 文件内容</param>
    /// <returns>文章引言，若不存在，则返回空字符串</returns>
    public static string GetArticleQuote(string markdown)
    {
        if (markdown.Contains("<!-- more -->") != true)
        {
            return string.Empty;
        }

        string quote = string.Empty;
        MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
        YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        foreach (MarkdownObject item in doc)
        {
            if (item is HtmlBlock htmlBlock)
            {
                string html = markdown.Substring(htmlBlock.Span.Start, htmlBlock.Span.Length);
                if (html == "<!-- more -->")
                {
                    if (yamlBlock is not null)
                    {
                        quote = markdown[(yamlBlock.Span.End + 1)..htmlBlock.Span.Start].Trim();
                    }
                    else
                    {
                        quote = markdown[..htmlBlock.Span.Start].Trim();
                    }
                    break;
                }
            }
        }

        return quote;
    }

    /// <summary>
    /// 获取 Markdown 文章的字数
    /// </summary>
    /// <param name="markdown">Markdown 文档的字符串</param>
    /// <returns>文章字数</returns>
    public static int GetWordCount(string markdown)
    {
        StringBuilder stringBuilder = new(markdown);

        MarkdownDocument doc = Markdown.Parse(markdown, pipeline);

        foreach (MarkdownObject item in doc.Descendants())
        {
            if (item is YamlFrontMatterBlock or HtmlBlock)
            {
                IEnumerable<char> padding = Enumerable.Repeat('\a', item.Span.Length);
                string paddingString = new(padding.ToArray());

                stringBuilder.Remove(item.Span.Start, item.Span.Length);
                stringBuilder.Insert(item.Span.Start, paddingString);
            }
        }

        stringBuilder.Replace("\a", string.Empty);
        string plainText = Markdown.ToPlainText(stringBuilder.ToString(), pipeline: pipeline);
        int count = WordCountHelper.GetWordCountFromString(plainText);
        return count;
    }

    /// <summary>
    /// 检查指定的 Markdown 文档是否包含指定的 CSS 类名
    /// </summary>
    /// <param name="markdown">Markdown 文档的字符串</param>
    /// <param name="className">类名</param>
    /// <returns>若有指定的类名，则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
    public static bool IsContainHtmlClass(string markdown, string className)
    {
        string html = Markdown.ToHtml(markdown, pipeline);

        HtmlParser parser = new();
        using IHtmlDocument document = parser.ParseDocument(html);
        IHtmlCollection<IElement> target = document.QuerySelectorAll($".{className}");

        return target.Length != 0;
    }
}
