using AnEoT.Vintage.Common.Models.HomePage;
using AnEoT.Vintage.Common.Helpers;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AnEoT.Vintage.Tool;

/// <summary>
/// 移除“Forceflash”组件的类
/// </summary>
internal static class ForceflashRemoval
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
    /// 移除 forceflash.md 文件，并移除其他文件对其的引用
    /// </summary>
    /// <param name="webRootPath">"wwwroot"文件夹的路径</param>
    public static Task RemoveForceFlash(string webRootPath)
    { 
        DirectoryInfo wwwRootDirectoryInfo = new(webRootPath);

        #region 第一步：移除 forceflash.md
        Console.WriteLine("第一步：移除 forceflash.md 文件");

        FileInfo? forceflashFile = wwwRootDirectoryInfo.GetFiles().Where(file => file.Name.Contains("forceflash",
                                                                                             StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        if (forceflashFile is null)
        {
            Console.WriteLine("forceflash.md 本就不存在，跳过此过程。");
        }
        else
        {
            //forceflashFile.Delete();
            Console.WriteLine("已删除 forceflash.md 文件。");
        }
        #endregion

        #region 第二步：寻找其他文件对 forceflash.md 的引用，并删除这些引用
        Console.WriteLine("第二步：移除对 forceflash.md 的引用");

        foreach (FileInfo item in wwwRootDirectoryInfo.EnumerateFiles("*.md"))
        {
            string markdown = File.ReadAllText(item.FullName);

            MarkdownDocument document = Markdown.Parse(markdown, pipeline);
            IEnumerable<LinkInline> linkInlines = document.Descendants<LinkInline>();
            YamlFrontMatterBlock? yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (yamlBlock is not null)
            {
                HomePageInfo homepageInfo = MarkdownHelper.GetFromFrontMatter<HomePageInfo>(markdown);
            }

            foreach (LinkInline link in linkInlines)
            {
                Console.WriteLine(markdown.Substring(link.Span.Start, link.Span.Length));
            }
        }
        #endregion

        return Task.CompletedTask;
    }
}
