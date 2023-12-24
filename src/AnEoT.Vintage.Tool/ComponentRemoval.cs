using AnEoT.Vintage.Common.Models.HomePage;
using AnEoT.Vintage.Common.Helpers;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Text;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace AnEoT.Vintage.Tool
{
    /// <summary>
    /// 移除主项目指定组件的类
    /// </summary>
    internal static class ComponentRemoval
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UsePreciseSourceLocation()
            .UseAdvancedExtensions()
            .UseListExtras()
            .UseEmojiAndSmiley(true)
            .UseYamlFrontMatter()
            .Build();

        /// <summary>
        /// 移除指定组件，并移除其他文件对其的引用
        /// </summary>
        /// <param name="componentName">指定组件的名称</param>
        /// <param name="webRootPath">"wwwroot"文件夹的路径</param>
        public static void Remove(string componentName, string webRootPath)
        {
            Console.WriteLine($"当前步骤：移除 {componentName}");
            Console.WriteLine();

            DirectoryInfo wwwRootDirectoryInfo = new(webRootPath);

            #region 第一步：移除指定组件
            Console.WriteLine($"第一步：移除 {componentName}.md 文件...");

            FileInfo? componentFile = wwwRootDirectoryInfo.GetFiles()
                .Where(file => file.Name.Contains(componentName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (componentFile is null)
            {
                Console.WriteLine($"{componentName}.md 本就不存在，跳过此过程。");
            }
            else
            {
                componentFile.Delete();
                Console.WriteLine($"已删除 {componentFile.FullName}。");
            }
            #endregion

            #region 第二步：寻找并删除其他文件对指定组件的引用
            Console.WriteLine();
            Console.WriteLine($"第二步：移除所有文件对 {componentName}.md 的引用...");

            RemoveRecursively(componentName, wwwRootDirectoryInfo);
            #endregion
        }

        /// <summary>
        /// 遍历指定的文件夹（包括子文件夹），然后删除文件夹中的 Markdown 文件对指定组件的引用
        /// </summary>
        /// <param name="directory">目标目录</param>
        private static void RemoveRecursively(string componentName, DirectoryInfo directory)
        {
            //目标：当前文件夹中的文件
            foreach (FileInfo file in directory.EnumerateFiles("*.md"))
            {
                RemoveComponentReference(componentName, file);
            }

            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
            {
                //目标：子文件夹中的文件
                foreach (FileInfo file in subDirectory.EnumerateFiles("*.md"))
                {
                    RemoveComponentReference(componentName, file);
                }

                //递归：对子文件夹的子文件夹进行操作
                RemoveRecursively(componentName, subDirectory);
            }
        }

        /// <summary>
        /// 移除 Markdown 文件中对指定组件的引用
        /// </summary>
        /// <param name="file">目标文件信息</param>
        private static void RemoveComponentReference(string componentName, FileInfo file)
        {
            string markdown = File.ReadAllText(file.FullName);

            MarkdownDocument document = Markdown.Parse(markdown, pipeline);
            YamlFrontMatterBlock? yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            if (yamlBlock is not null && MarkdownHelper.TryGetFromFrontMatter(markdown, out HomePageInfo homePageInfo))
            {
                List<HomePageProjectsItem> homePageProjectsItems = homePageInfo.Projects.ToList();
                int removedCount = homePageProjectsItems.RemoveAll(projectsItem => projectsItem.Link == componentName);

                if (removedCount > 0)
                {
                    HomePageInfo newHomePageInfo = homePageInfo with { Projects = homePageProjectsItems };

                    ISerializer serializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();
                    string yaml = serializer.Serialize(newHomePageInfo);

                    string newLine = Environment.NewLine;

                    StringBuilder stringBuilder = new(markdown);
                    stringBuilder.Remove(yamlBlock.Span.Start, yamlBlock.Span.Length);
                    stringBuilder.Insert(0, $"---{newLine}{yaml}{newLine}---");
                    string newMarkdown = stringBuilder.ToString();

                    using StreamWriter writer = File.CreateText(file.FullName);
                    writer.Write(newMarkdown);

                    Console.WriteLine($"已移除 {file.FullName} 的 Front Matter 中，对 {componentName} 的引用。");
                }
            }

            List<string> markdownFileLines = new(File.ReadAllLines(file.FullName));
            List<int> linesToBeRemoved = new(2);

            foreach (string line in markdownFileLines)
            {
                if (line.Contains(componentName, StringComparison.OrdinalIgnoreCase))
                {
                    int currentLine = markdownFileLines.IndexOf(line);
                    linesToBeRemoved.Add(currentLine);
                }
            }

            if (linesToBeRemoved.Count != 0)
            {
                foreach (int lineIndex in linesToBeRemoved)
                {
                    markdownFileLines.RemoveAt(lineIndex);
                }

                File.WriteAllLines(file.FullName, markdownFileLines);
                Console.WriteLine($"已移除 {file.FullName} 中对 {componentName} 的引用。");
            }
        }
    }
}
