using System.CommandLine;

namespace AnEoT.Vintage.Tool;

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
                result.ErrorMessage = "指定的目录不存在。";
                return;
            }
        });
        
        Command removeForceFlashCommand = new("remove-unnecessary-component", "移除不需要的文件及其引用。");
        removeForceFlashCommand.AddAlias("ruc");
        removeForceFlashCommand.AddOption(webRootPathOption);
        removeForceFlashCommand.SetHandler(wwwrootPath =>
        {
            //TODO: Add more...
            ForceflashRemoval.RemoveForceFlash(wwwrootPath);
        }, webRootPathOption);

        rootCommand.AddCommand(removeForceFlashCommand);

        return rootCommand.InvokeAsync(args).Result;
    }
}