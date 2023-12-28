using System.Buffers;
using AspNetStatic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 协助在程序中转换 WebP 图像到 JPG 图像的类
/// </summary>
public class WebPContentConverter : IBinOptimizer
{
    /// <inheritdoc/>
    public BinOptimizerResult Execute(byte[] content, BinResource resource, string outFilePathname)
    {
        using MemoryStream stream = new(content.Length * 2);
        using Image image = Image.Load(content);

        image.Mutate(x => x.BackgroundColor(Color.White));
        image.SaveAsJpeg(stream);

        byte[] optimizedContent = stream.ToArray();
        return new BinOptimizerResult(optimizedContent);
    }
}
