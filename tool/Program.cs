using System.CommandLine;

namespace AnEoT.Vintage.Tool;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        Option<string?> removeForceFlashOption = new("--remove-forceflash",
            "移除 forceflash.md 文件，并移除其他文件对其的引用");

        RootCommand rootCommand = new("AnEoT.Vintage 项目工具");
        rootCommand.AddOption(removeForceFlashOption);

        rootCommand.SetHandler(projectPath =>
        {
            
        }, removeForceFlashOption);

        return await rootCommand.InvokeAsync(args).ConfigureAwait(true);
    }
}