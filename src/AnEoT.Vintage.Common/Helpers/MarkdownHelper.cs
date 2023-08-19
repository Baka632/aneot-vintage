using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

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

        string? quote = null;
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

        return quote ?? string.Empty;
    }
}
