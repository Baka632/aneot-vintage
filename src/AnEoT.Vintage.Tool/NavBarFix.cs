using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private static void ModifyRecursively(DirectoryInfo directory)
        {
            //目标：当前文件夹中的文件
            foreach (FileInfo file in directory.EnumerateFiles("*.html"))
            {
                
            }

            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
            {
                //目标：子文件夹中的文件
                foreach (FileInfo file in subDirectory.EnumerateFiles("*.html"))
                {
                    
                }

                //递归：对子文件夹的子文件夹进行操作
                ModifyRecursively(subDirectory);
            }
        }
    }
}
