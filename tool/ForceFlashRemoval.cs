namespace AnEoT.Vintage.Tool;

/// <summary>
/// 移除“Forceflash”组件的类
/// </summary>
internal sealed class ForceflashRemoval
{
    private readonly string _webRootPath;

    /// <summary>
    /// 使用指定的参数构造<seealso cref="ForceflashRemoval"/>的新实例
    /// </summary>
    /// <param name="webRootPath">目标"wwwroot"文件夹的路径</param>
    /// <exception cref="IOException">目录<paramref name="webRootPath"/>不存在</exception>
    public ForceflashRemoval(string webRootPath)
    {
        if (!Directory.Exists(webRootPath))
        {
            throw new IOException($"目录 {webRootPath} 不存在");
        }

        _webRootPath = webRootPath;
    }

    public Task RemoveForceFlash()
    {
        //TODO: Add implementation...
        Console.WriteLine(_webRootPath);
        return Task.CompletedTask;
    }
}
