using Markdig;
using Markdig.Extensions.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;
using Markdig.Syntax;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Html;
using Markdig.Renderers.Normalize;

namespace AnEoT.Vintage.Helper
{
    /// <summary>
    /// 为Markdown处理提供通用操作
    /// </summary>
    internal partial class MarkdownHelper
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

        /// <summary>
        /// 获取由Markdown中Front Matter转换而来的模型
        /// </summary>
        /// <param name="markdown">Markdown文件内容</param>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns>转换得到的模型</returns>
        public T GetFrontMatter<T>(string markdown)
        {
            MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
            YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlBlock is not null)
            {
                string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
                using StringReader input = new(yaml);

                Parser yamlParser = new(input);
                yamlParser.Consume<StreamStart>();
                yamlParser.Consume<DocumentStart>();

                IDeserializer yamlDes = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                T postInfo = yamlDes.Deserialize<T>(yamlParser);
                yamlParser.Consume<DocumentEnd>();

                return postInfo;
            }
            else
            {
                throw new ArgumentException("无法通过指定的参数解析出模型，Markdown可能没有Front Matter信息");
            }
        }

        /// <summary>
        /// 将Markdown中的Uri更改为绝对Uri
        /// </summary>
        /// <param name="markdown">Markdown文件内容</param>
        /// <param name="post">期刊名称</param>
        /// <param name="urlHelper">Uri帮助器</param>
        /// <returns>包含绝对Uri的Markdown</returns>
        public string ReplaceUriAsAbsolute(string markdown, string? post, IUrlHelper urlHelper)
        {
            markdown = GetMarkdownHtmlLinkRegex()
                .Replace(markdown, "${Extension}.md)");

            markdown = GetMarkdownImageLinkRegex().Replace(markdown, (match) =>
            {
                string matchedUri = match.Groups["Uri"].Value;
                string matchedImageDesc = match.Groups["ImageDesc"].Value;

                string urlPart = urlHelper.Content($"~/aneot/posts/{post}/{matchedUri}");
                return $"![{matchedImageDesc}]({urlPart})";
            });

            markdown = GetMarkdownOtherLinkRegex().Replace(markdown, (match) =>
            {
                string matchedUri = match.Groups["Uri"].Value;
                string matchedTitle = match.Groups["Title"].Value;

                string urlPart = urlHelper.Content(string.IsNullOrWhiteSpace(post)
                    ? $"~/posts/{matchedUri}"
                    : $"~/posts/{post}/{matchedUri}");
                return $" [{matchedTitle}]({urlPart})";
            });

            return markdown;
        }

        [GeneratedRegex(@"(?<Extension>\[.*\]\(.*)\.html\)")]
        private static partial Regex GetMarkdownHtmlLinkRegex();

        [GeneratedRegex(@"[^!]\[(?<Title>.*)\]\((?<Uri>.*)\)")]
        private static partial Regex GetMarkdownOtherLinkRegex();

        [GeneratedRegex(@"!\[(?<ImageDesc>.*)\]\((?<Uri>.*)\)")]
        private static partial Regex GetMarkdownImageLinkRegex();
    }
}
