using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TotalCommander.Classes
{
    static class DirectoryParams
    {
        public static bool isRightWindow = true;

        public static ConsoleKeyInfo? keyInfos = null;
        /// <summary>
        /// Відображає файли по лівій частині екрану
        /// </summary>
        /// <returns>Повертає шлях</returns>
        public static string ShowAllFiles(DirectoryInfo dir, int counter = 0)
        {
            int num;
            //  Запис у масив усіх данних про файли та папки
            IEnumerable<string> nameTake;
            Console.BackgroundColor = ConsoleColor.Blue;
            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            if (counter < 0)
            {
                counter = 0;
            }

            int skipped = 0;
            int CounterX = 6;
            int CounterY = 2;

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            //  Відображення меню для вибору папки чи файлу
            do
            {
                int c = skipped;
                bool flag = false;
                int a = skipped;
                CounterX = 6;
                CounterY = 2;
                string[] names = new string[directories.Length + files.Length + 1];
                for (int i = 0; i < directories.Length; i++)
                {
                    names[i] = directories[i].Name;
                }
                for (int i = 0; i < files.Length; i++)
                {
                    names[i + directories.Length] = files[i].Name;
                }
                names[directories.Length + files.Length] = "..";
                nameTake = names;
                if (nameTake.Count() > 20)
                {
                    foreach (var str in nameTake.Skip(skipped).Take(20))
                    {
                        if (counter == a++)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.SetCursorPosition(CounterX, CounterY++);
                        Console.WriteLine(str);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                else
                {
                    foreach (var str in nameTake)
                    {
                        if (counter == a++)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.SetCursorPosition(CounterX, CounterY++);
                        if (str.Length < 48)
                        {
                            Console.WriteLine(str);
                        }
                        else
                        {
                            for (int i = 0; i < 48; i++)
                            {
                                Console.Write(str[i]);
                            }
                            Console.Write("..");
                        }
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }

                string pattern = @".*\.txt";
                Params.ClearRight();
                if (!names[counter].Contains("..") && isRightWindow)
                {
                    if (Regex.IsMatch(names[counter], pattern))
                    {
                        if (File.Exists(dir.FullName + @"\" + names[counter]))
                        {
                            using (FileStream fs = new FileStream(dir.FullName + @"\" + names[counter], FileMode.Open, FileAccess.Read))
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    try
                                    {
                                        Params.ReadAndShowFileRight(sr);
                                    }
                                    catch { }
                                }
                            }
                        }
                    } 
                    else if (Directory.Exists(dir.FullName + @"\" + names[counter]))
                    {
                        DirectoryInfo dInfo = new DirectoryInfo(dir.FullName + @"\" + names[counter]);
                        try
                        {
                            num = ShowAllFilesRight(dInfo);
                                if (num > 0)
                                {
                                    counter++;
                                    flag = true;
                                }
                                else if (num < 0 && num != -5)
                                {
                                    counter--;
                                    if (counter < 0)
                                    {
                                        counter = directories.Length + files.Length;
                                        if (counter > 20) 
                                        {
                                            skipped = counter - 20 + 1;
                                            Params.Clear();
                                        }
                                    }
                                    flag = true;
                                    
                                }
                                else if (num == 0)
                                {
                                    break;
                                }
                             
                            }
                        catch { }
                
                    }
                    
                }
                    Console.BackgroundColor = ConsoleColor.Blue;

                if (!flag) 
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(0,23);
                    Console.Write(": ");
                    if (keyInfos == null)
                    {
                        keyInfo = Console.ReadKey();
                    }
                    else 
                    {
                        keyInfo = (ConsoleKeyInfo)keyInfos;
                        keyInfos = null;
                    }
                    Console.BackgroundColor = ConsoleColor.Blue;


                    if (keyInfo.Key != ConsoleKey.UpArrow && keyInfo.Key != ConsoleKey.DownArrow)
                    {
                        switch (keyInfo.Key) 
                        {
                            case ConsoleKey.D1:
                                {
                                    Params.Clear();
                                    Params.ClearRight();
                                    string lastDisk = Drive.driveName;
                                    DriveInfo d = new DriveInfo( Drive.ChangeDrive());
                                    if (d.IsReady)
                                    {
                                        return d.Name;
                                    }
                                    else 
                                    {
                                        Drive.driveName = lastDisk;
                                        return lastDisk;
                                    }
                                }
                            case ConsoleKey.D2: 
                                { 
                                   return CreatedDirectory(dir.FullName);
                                }
                            case ConsoleKey.D3: 
                                {
                                    return FileCreated(dir.FullName);
                                }
                            case ConsoleKey.D4: 
                                {
                                    return DeleteDirectory(dir.FullName + @"\" + names[counter]);
                                }
                            case ConsoleKey.D5: 
                                {
                                    return DeleteFile(dir.FullName + @"\" +names[counter]);
                                }
                            case ConsoleKey.D6: 
                                {
                                    CopyFiles(dir.FullName + @"\" + names[counter]);
                                    return dir.FullName;
                                }
                            case ConsoleKey.D7: 
                                {
                                   return MoveFiles(dir.FullName + @"\" + names[counter]);
                                }
                            case ConsoleKey.D8: 
                                {
                                    return "END";
                                }
                        }
                    }

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            {
                                if (counter > 0)
                                {
                                counter--;
                                    if (skipped > 0)
                                    {
                                        Params.Clear();
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        skipped--;
                                        c--;
                                    }
                                }
                                else
                                {
                                    Params.Clear();
                                    Console.BackgroundColor = ConsoleColor.Blue;
                                    counter = directories.Length + files.Length;
                                    if (counter > 20)
                                    {
                                        skipped = counter + 1 - 20;
                                    }
                                }

                               

                                break;
                            }
                        case ConsoleKey.DownArrow:
                            {
                                if (counter < directories.Length + files.Length)
                                {
                                    counter++;
                                    if (counter >= 20 + skipped)
                                    {
                                        skipped++;
                                        c++;
                                    }
                                }
                                else
                                {
                                    Params.Clear();
                                    Console.BackgroundColor = ConsoleColor.Blue;
                                    skipped = 0;
                                    counter = 0;
                                }
                                break;
                            }
                    }

                }
                if (counter > 19)
                {
                    Params.Clear();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    skipped = counter - 19;
                }
                else 
                {
                    skipped = 0;
                }

                }
                while (keyInfo.Key != ConsoleKey.Enter) ;
            //  Кінець меню
                Console.BackgroundColor = ConsoleColor.Black;



                if (counter < directories.Length)
                {
                    return directories[counter].FullName;
                }
                else
                {
                    if (counter == directories.Length + files.Length)
                    {

                        if (dir.FullName == Drive.driveName)
                        {
                            return dir.FullName;
                        }
                        else
                        {
                            return dir.Parent.FullName;
                        }
                    }
                    counter -= directories.Length;
                    return files[counter].FullName;
                }
        }

        /// <summary>
        /// Відображає файли по правій частині екрану
        /// </summary>
        /// <returns>Повертає шлях</returns>
        public static int ShowAllFilesRight(DirectoryInfo dir, int skipp = 0)
        { 
            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            int skipped = skipp;
           
          

                string[] names = new string[directories.Length + files.Length + 1];
                for (int i = 0; i < directories.Length; i++)
                {
                    names[i] = directories[i].Name;
                }
                for (int i = 0; i < files.Length; i++)
                {
                    names[i + directories.Length] = files[i].Name;
                }

                IEnumerable<string> nameTake = names;
            //  Меню
            //  Якщо в масиві більше 20 елементів 
            if (nameTake.Count() > 20)
            {
                while (true) 
                {
                    Params.ClearRight();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    int CounterX = (98 / 2) + 9;
                    int CounterY = 2;
                    skipped = skipp;
                foreach (var str in nameTake.Skip(skipped).Take(20))
                {
                    Console.SetCursorPosition(CounterX, CounterY++);

                    if (str != null)
                    {
                        if (str.Length < 48)
                        {
                            Console.WriteLine(str);
                        }
                        else
                        {
                            for (int i = 0; i < 48; i++)
                            {
                                Console.Write(str[i]);
                            }
                            Console.Write("..");
                        }
                    }
                    else
                    {
                        CounterY--;
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.S:
                        {
                            if (20 + skipp < nameTake.Count() - 1)
                            {
                                skipp++;
                            }
                            break;
                        }
                    case ConsoleKey.W:
                        {
                            if (skipp > 0)
                            {
                                skipp--;
                            }
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            return -1;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            return 1;
                        }
                    case ConsoleKey.Enter:
                        {
                            return 0;
                        }
                    default: 
                            {
                                keyInfos = keyInfo;
                                return -5;
                            }
                }
                    Console.BackgroundColor = ConsoleColor.Black;

                }
            }
            //  Якщо в масиві менше 20 елементів 
            else
            {
                Params.ClearRight();
                Console.BackgroundColor = ConsoleColor.Blue;
                int CounterX = (98 / 2) + 9;
                int CounterY = 2;
                foreach (var str in nameTake)
                {
                    Console.SetCursorPosition(CounterX, CounterY++);

                    if (str != null)
                    {
                        if (str.Length < 48)
                        {
                            Console.WriteLine(str);
                        }
                        else
                        {
                            for (int i = 0; i < 48; i++)
                            {
                                Console.Write(str[i]);
                            }
                            Console.Write("..");
                        }
                    }
                    else
                    {
                        CounterY--;
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }


            Console.BackgroundColor = ConsoleColor.Black;
            return -5;
        }

        public static string CreatedDirectory(string path) 
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 23);
            Console.Write("Ведіть назву - ");
            string name = Console.ReadLine();
            Params.ClearLine(23);

            DirectoryInfo directory = new DirectoryInfo(path + @"\" + name);
            if (directory.Exists)
            {
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
                Console.WriteLine("Папка уже була створена!");
                Thread.Sleep(2000);
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
            }
            else 
            {
                directory.Create();
                Console.SetCursorPosition(0, 23);
                Console.WriteLine("Папку створено!");
                Thread.Sleep(2000);
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
            }
            Console.BackgroundColor = ConsoleColor.Blue;

            return directory.FullName;
        }

        public static string FileCreated(string path) 
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 23);
            Console.Write("Ведіть назву - ");
            string name = Console.ReadLine();
            Params.ClearLine(23);

            FileInfo file = new FileInfo(path + @"\" + name);
            if (file.Exists)
            {
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
                Console.WriteLine("Файл уже був створений!");
                Thread.Sleep(2000);
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
            }
            else
            {
                FileStream fs = file.Create();
                fs.Close();
                Console.SetCursorPosition(0, 23);
                Console.WriteLine("Файл створено!");
                Thread.Sleep(2000);
                Params.ClearLine(23);
                Console.SetCursorPosition(0, 23);
            }
            Console.BackgroundColor = ConsoleColor.Blue;

            return file.Directory.FullName;
        }

        public static string DeleteDirectory(string path) 
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            string parent = directory.Parent.FullName;
            try
            {
                if (directory.Exists)
                {
                    directory.Delete(true);
                }
            }
            catch { }
            return parent;
        }

        public static string DeleteFile(string path) 
        {
            FileInfo file = new FileInfo(path);
            string parent = file.Directory.FullName;
            try
            {
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch { }
            return parent;
        }

        public static string MoveFiles(string path) 
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Params.ClearLine(23);
            Console.SetCursorPosition(0, 23);
            Console.Write("Ведіть шлях - ");
            string newPath = Console.ReadLine();
            DirectoryInfo dir = new DirectoryInfo(newPath);

            if (dir.Exists) 
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        DirectoryInfo dir2 = new DirectoryInfo(path);
                        dir2.MoveTo(dir.FullName + @"\" + dir2.Name);
                    }
                    catch { }
                    finally 
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Params.ClearLine(23);
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                }
                else if(File.Exists(path))
                {
                    try
                    {
                        FileInfo file = new FileInfo(path);
                        file.MoveTo(dir.FullName + @"\" + file.Name);
                    }
                    catch { }
                    finally 
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Params.ClearLine(23);
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Params.ClearLine(23);
                Console.BackgroundColor = ConsoleColor.Blue;
                return newPath;
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Params.ClearLine(23);
            Console.BackgroundColor = ConsoleColor.Blue;
            return Directory.GetParent(path).FullName;
        }

        public static void CopyFiles(string path) 
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.CopyDir();
            }
            else
            {
                int ind = 1;
                FileInfo file = new FileInfo(path);
                if (file.Extension == ".txt") 
                {
                string[] elements = Directory.GetFiles(file.Directory.FullName)
                    .Where(f => f.Contains("-copy"))
                    .Select(f => f).ToArray();
                ind = elements.Count() + 1;
                string nameFile = file.Name;
                int counter = nameFile.LastIndexOf('.');
                nameFile = nameFile.Insert(counter, "-copy [" + ind + "]");

                    
                    string text = null;
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) 
                    {
                        using (StreamReader sr = new StreamReader(fs)) 
                        {
                            text = sr.ReadToEnd();
                        }
                    }

                    using (FileStream fs = new FileStream(file.Directory.FullName + @"\" + nameFile, 
                        FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(text);
                        }
                    }
                }
                
            }
        }
    }
}
