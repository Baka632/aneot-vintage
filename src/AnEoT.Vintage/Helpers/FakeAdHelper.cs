using System.Text.Json;
using AnEoT.Vintage.Models;

namespace AnEoT.Vintage.Helpers;

/// <summary>
/// 为生成「泰拉广告」提供帮助方法的类
/// </summary>
public static class FakeAdHelper
{
    private static bool isDataPrepared;
    private static string? webRootPath;
    private static Dictionary<string, FakeAdConfiguration>? fakeAdConfigurations;

    /// <summary>
    /// 使用指定的参数准备数据源，必须调用此方法才能进行获取「泰拉广告」
    /// </summary>
    /// <param name="wwwRootPath">“wwwroot”文件夹所在路径</param>
    public static async void PrepareData(string wwwRootPath)
    {
        if (string.IsNullOrWhiteSpace(wwwRootPath))
        {
            throw new ArgumentException($"“{nameof(wwwRootPath)}”不能为 null 或空白。", nameof(wwwRootPath));
        }

        FileInfo adJsonFile = new(Path.Combine(wwwRootPath, "fake-ads", "ads.json"));

        FileStream utf8Json = adJsonFile.OpenRead();
        Dictionary<string, FakeAdConfiguration>? adConfig = await JsonSerializer.DeserializeAsync<Dictionary<string, FakeAdConfiguration>>(utf8Json);
        
        fakeAdConfigurations = adConfig;

        webRootPath = wwwRootPath;
        isDataPrepared = true;
    }

    /// <summary>
    /// 随机获取一个「泰拉广告」
    /// </summary>
    public static FakeAdInfo RollFakeAd(bool convertWebp)
    {
        if (isDataPrepared is not true || fakeAdConfigurations is null || string.IsNullOrWhiteSpace(webRootPath))
        {
            throw new InvalidOperationException($"必须先调用 {nameof(PrepareData)} 方法才能进行此操作");
        }

        FakeAdInfo fakeAdInfo = new();

        float cumulativeProbability = 0;
        Random random = new();
        foreach (var item in fakeAdConfigurations)
        {
            cumulativeProbability += item.Value.probability;

            if (random.NextSingle()  < cumulativeProbability)
            {
                FakeAdConfiguration selectedAd = item.Value;
                int index = (int)Math.Floor(random.NextDouble() * selectedAd.files!.Length);
                string fileName = selectedAd.files[index];
                FileInfo adInfoFile = new(Path.Combine(webRootPath, "fake-ads", $"{fileName}.txt"));

                fakeAdInfo = fakeAdInfo with
                {
                    AdImageLink = convertWebp switch
                    {
                        true => $"{fileName}.jpg",
                        false => $"{fileName}.webp",
                    }
                };

                using StreamReader reader = adInfoFile.OpenText();
                for (int i = 0; reader.Peek() != -1; i++)
                {
                    string? line = reader.ReadLine();

                    fakeAdInfo = i switch
                    {
                        0 => fakeAdInfo with { AdText = line },
                        1 => fakeAdInfo with { AdAbout = line },
                        2 => fakeAdInfo with { AboutLink = line},
                        3 => fakeAdInfo with { AdLink = line },
                        _ => throw new NotImplementedException(),
                    };
                }
                return fakeAdInfo;
            }
        }

        throw new NotImplementedException();
    }

    internal struct FakeAdConfiguration
    {
        public float probability { get; set; }
        public string[]? files { get; set; }
    }
}