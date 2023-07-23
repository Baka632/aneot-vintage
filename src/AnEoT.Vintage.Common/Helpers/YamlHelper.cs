using System.Diagnostics.CodeAnalysis;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;

namespace AnEoT.Vintage.Common.Helpers
{
    /// <summary>
    /// 为 YAML 处理提供通用操作
    /// </summary>
    public static class YamlHelper
    {
        /// <summary>
        /// 从 YAML 字符串中反序列化出指定的对象
        /// </summary>
        /// <typeparam name="T">反序列化出的对象</typeparam>
        /// <param name="yaml">YAML 字符串</param>
        /// <returns>指定的对象实例</returns>
        [RequiresDynamicCode("此方法调用了不支持 IL 裁剪的 YamlDotNet.Serialization.DeserializerBuilder.DeserializerBuilder()")]
        public static T ReadYaml<T>(string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                throw new ArgumentException("参数为 null 或空白字符串", nameof(yaml));
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

        /// <summary>
        /// 尝试从 YAML 字符串中反序列化出指定的对象
        /// </summary>
        /// <typeparam name="T">反序列化出的对象</typeparam>
        /// <param name="yaml">YAML 字符串</param>
        /// <param name="result">指定的对象实例</param>
        /// <returns>指示转换是否成功的值</returns>
        [RequiresDynamicCode("此方法调用了不支持 IL 裁剪的 YamlDotNet.Serialization.DeserializerBuilder.DeserializerBuilder()")]
        public static bool TryReadYaml<T>(string yaml, [MaybeNullWhen(false)] out T result)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                result = default;
                return false;
            }

            T obj;
            try
            {
                StringReader input = new(yaml);
                Parser yamlParser = new(input);
                yamlParser.Consume<StreamStart>();
                yamlParser.Consume<DocumentStart>();

                IDeserializer yamlDes = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                obj = yamlDes.Deserialize<T>(yamlParser);
                yamlParser.Consume<DocumentEnd>();
            }
            catch
            {
                result = default;
                return false;
            }

            result = obj;
            return true;
        }
    }
}
