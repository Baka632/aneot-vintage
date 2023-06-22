using Markdig;
using Markdig.Extensions.Yaml;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;
using Markdig.Syntax;

namespace AnEoT.Vintage.Helper
{
    /// <summary>
    /// 为Markdown处理提供通用操作
    /// </summary>
    internal static class MarkdownHelper
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
        public static T? GetFrontMatter<T>(string markdown)
        {
            MarkdownDocument doc = Markdown.Parse(markdown, pipeline);
            YamlFrontMatterBlock? yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlBlock is not null)
            {
                string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
                T? model = ReadYaml<T>(yaml);

                return model;
            }
            else
            {
                throw new ArgumentException("无法通过指定的参数解析出模型，Markdown可能没有Front Matter信息");
            }
        }

        /// <summary>
        /// 从Yaml字符串中反序列化出指定的对象
        /// </summary>
        /// <typeparam name="T">反序列化出的对象</typeparam>
        /// <param name="yaml">Yaml字符串</param>
        /// <returns>指定的对象实例</returns>
        public static T? ReadYaml<T>(string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                return default;
            }

            StringReader input = new(yaml);
            Parser yamlParser = new(input);
            yamlParser.Consume<StreamStart>();
            yamlParser.Consume<DocumentStart>();

            IDeserializer yamlDes = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            T obj = yamlDes.Deserialize<T>(yamlParser);
            yamlParser.Consume<DocumentEnd>();
            return obj;
        }
    }
}
