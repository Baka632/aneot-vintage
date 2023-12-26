using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AnEoT.Vintage.Tool
{
    /// <summary>
    /// 为修改导航栏提供帮助的类
    /// </summary>
    internal static class NavBarFix
    {
        public static void FixNavBar(string staticContentPath)
        {
            Console.WriteLine("正在修改导航栏以适配 GitHub Pages...");
            DirectoryInfo staticContentDirectory = new(staticContentPath);

            ModifyRecursively(staticContentDirectory);
        }

        private static void ModifyRecursively(DirectoryInfo directory)
        {
            //目标：当前文件夹中的文件
            foreach (FileInfo file in directory.EnumerateFiles("*.html"))
            {
                ModifyNavBarLinks(file);
            }

            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
            {
                //目标：子文件夹中的文件
                foreach (FileInfo file in subDirectory.EnumerateFiles("*.html"))
                {
                    ModifyNavBarLinks(file);
                }

                //递归：对子文件夹的子文件夹进行操作
                ModifyRecursively(subDirectory);
            }
        }

        private static void ModifyNavBarLinks(FileInfo file)
        {
            string html = File.ReadAllText(file.FullName);
            HtmlParser parser = new();

            using IHtmlDocument document = parser.ParseDocument(html);
            IEnumerable<IElement> navBarLinks = document.All
                .Where(element => element.TagName.ToUpperInvariant() is "A" or "IMG" or "LINK" or "SCRIPT");

            bool isModified = false;

            foreach (IElement item in navBarLinks)
            {
                switch (item)
                {
                    //额外加大括号的原因是防止变量外溢而影响其他case块
                    case IHtmlAnchorElement anchor:
                        {
                            string? originalHref = anchor.GetAttribute("href");
                            if (originalHref is not null && originalHref.StartsWith('/') && originalHref.Contains("/aneot-vintage") is not true)
                            {
                                anchor.SetAttribute("href", $"/aneot-vintage{originalHref}");
                                isModified = true;
                            }
                        }
                        break;
                    case IHtmlImageElement image:
                        {
                            string? originalSrc = image.GetAttribute("src");
                            if (originalSrc is not null && originalSrc.StartsWith('/') && originalSrc.Contains("/aneot-vintage") is not true)
                            {
                                image.SetAttribute("src", $"/aneot-vintage{originalSrc}");
                                isModified = true;
                            }
                        }
                        break;
                    case IHtmlLinkElement link:
                        {
                            string? originalLink = link.GetAttribute("href");
                            if (originalLink is not null && originalLink.StartsWith('/') && originalLink.Contains("/aneot-vintage") is not true)
                            {
                                link.SetAttribute("href", $"/aneot-vintage{originalLink}");
                                isModified = true;
                            }
                        }
                        break;
                    case IHtmlScriptElement script:
                        {
                            string? originalSrc = script.GetAttribute("src");
                            if (originalSrc is not null && originalSrc.StartsWith('/') && originalSrc.Contains("/aneot-vintage") is not true)
                            {
                                script.SetAttribute("src", $"/aneot-vintage{originalSrc}");
                                isModified = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (isModified)
            {
                using StreamWriter writer = File.CreateText(file.FullName);

                PrettyMarkupFormatter formatter = new();
                document.ToHtml(writer, formatter);

                Console.WriteLine($"已修改文件 {file.FullName}");
            }
        }
    }
}
