namespace AnEoT.Vintage.Models;

/// <summary>
/// 为表示期刊文件夹的 <see cref="DirectoryInfo"/> 提供比较方法，以用于排序。
/// </summary>
public sealed class VolumeDirectoryOrderComparer : IComparer<DirectoryInfo>
{
    /// <inheritdoc/>
    public int Compare(DirectoryInfo? x, DirectoryInfo? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }
        else if (y is null)
        {
            return 1;
        }

        return string.CompareOrdinal(x.Name, y.Name);
    }
}
