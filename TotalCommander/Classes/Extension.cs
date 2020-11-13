using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TotalCommander.Classes
{
    static class Extension
    {
        public static void CopyDir(this DirectoryInfo dir, DirectoryInfo? dir2 = null) 
        {
            
            string[] strs = dir.GetDirectories()
                .Where(t => t.Name.Contains("-copy"))
                .Select(t => t.Name).ToArray();
            int ind = strs.Count() + 1;
            DirectoryInfo directory;
            if (dir2 == null)
            {
                directory = new DirectoryInfo(dir.FullName + "_copy [" + ind++ + "]");
               
            }
            else 
            {
                directory = new DirectoryInfo(dir2.FullName + @"\" + dir.Name + "_copy [" + ind++ + "]");
            }
            directory.Create();

            if (dir.GetDirectories().Count() + dir.GetFiles().Count() <= 0) 
            {
                return;
            }

            foreach (var item in dir.GetFiles()) 
            {
                try
                {
                    string text;
                    using (FileStream fs = new FileStream(item.FullName, FileMode.Open, FileAccess.Read)) 
                    {
                        using (StreamReader sr = new StreamReader(fs)) 
                        {
                           text = sr.ReadToEnd();
                        }
                    }

                    using (FileStream fs = new FileStream(directory.FullName + @"\" +item.Name, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(text);
                        }
                    }
                }
                catch { }
            }

            if (dir.GetDirectories().Count() <= 0)
            {
                return;
            }
            foreach (var item in dir.GetDirectories()) 
            {
               CopyDir(item, directory);
            }
        }



    }
}
