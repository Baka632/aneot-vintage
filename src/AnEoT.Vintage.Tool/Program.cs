using System.CommandLine;

namespace AnEoT.Vintage.Tool;

[Obsolete("现在不再使用此程序。")]
internal sealed class Program
{
    static int Main(string[] args)
    {
        RootCommand rootCommand = new("AnEoT.Vintage 项目工具");

        Option<string> webRootPathOption = new(
            name: "--webroot-path",
            description: """目标"wwwroot"文件夹的路径""")
        { IsRequired = true };

        webRootPathOption.AddValidator(result =>
        {
            string? filePath = result.GetValueForOption(webRootPathOption);
            if (!Directory.Exists(filePath))
            {
                result.ErrorMessage = "指定的目录不存在，传入路径：{filePath}";
                return;
            }
        });
        
        Option<string> staticWebSiteOutputPathOption = new(
            name: "--static-content-path",
            description: "目标输出静态网页文件夹的路径")
        { IsRequired = true };

        staticWebSiteOutputPathOption.AddValidator(result =>
        {
            string? filePath = result.GetValueForOption(staticWebSiteOutputPathOption);
            if (!Directory.Exists(filePath))
            {
                result.ErrorMessage = "指定的目录不存在，传入路径：{filePath}";
                return;
            }
        });
        
        Command removeUnnecessaryComponentCommand = new("remove-unnecessary-component", "移除不需要的文件及其引用。");
        removeUnnecessaryComponentCommand.AddOption(webRootPathOption);
        removeUnnecessaryComponentCommand.SetHandler(wwwrootPath =>
        {
            Console.WriteLine("*** 现在不再使用此命令 ***");
            // Console.WriteLine($"工作目录：{wwwrootPath}");
            // Console.WriteLine("======");

            // 现在网页上已经没有 forceflash 组件了
            // ComponentRemoval.Remove("forceflash", wwwrootPath);
            
            // PWA 现在也是需要的东西了（）
            //Console.WriteLine("======");
            //ComponentRemoval.Remove("installpwa", wwwrootPath);
            //Console.WriteLine();
            //Console.WriteLine("操作成功完成。");
        }, webRootPathOption);
        
        /*Command fixGitHubPagesCommand = new("fix-github-pages", "修复静态网页在 GitHub Pages 中的问题。");
        fixGitHubPagesCommand.AddOption(staticWebSiteOutputPathOption);
        fixGitHubPagesCommand.SetHandler(staticWebSitePath =>
        {
            Console.WriteLine($"工作目录：{staticWebSitePath}");
            Console.WriteLine("======");
            GitHubPagesFix.FixFileForGitHubPages(staticWebSitePath);
            Console.WriteLine();
            Console.WriteLine("操作成功完成。");
        }, staticWebSiteOutputPathOption);*/

        rootCommand.AddCommand(removeUnnecessaryComponentCommand);
        // rootCommand.AddCommand(fixGitHubPagesCommand);

        return rootCommand.InvokeAsync(args).Result;
    }
}