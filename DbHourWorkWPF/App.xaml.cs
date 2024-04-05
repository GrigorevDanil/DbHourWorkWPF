using DbHourWorkWPF.Items;
using DbHourWorkWPF.Properties;
using DbHourWorkWPF.Utilities;
using DBHourWorkWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Database db;
        public static DatabaseService serviceDb;
        public static ItemUser Account;
        public static TextBlock txtNick;
        public static ImageBrush imgUser;
        public static INIManager manager = new INIManager("./Config.ini");

        public static string FindXamppMysqlBin()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady || drive.DriveType != DriveType.Fixed) continue;

                var binPath = FindXamppMysqlBinInDirectory(drive.RootDirectory);
                if (binPath != null)
                {
                    return binPath;
                }
            }

            return null;

            string FindXamppMysqlBinInDirectory(DirectoryInfo directoryInfo)
            {
                try
                {
                    foreach (var directory in directoryInfo.EnumerateDirectories())
                    {
                        if (directory.Name.Equals("bin", StringComparison.OrdinalIgnoreCase))
                        {
                            if (directory.Parent != null && directory.Parent.Name.Equals("mysql", StringComparison.OrdinalIgnoreCase))
                            {
                                if (directory.Parent.Parent != null && directory.Parent.Parent.Name.Equals("xampp", StringComparison.OrdinalIgnoreCase))
                                {
                                    return directory.FullName;
                                }
                            }
                        }

                        var binPath = FindXamppMysqlBinInDirectory(directory);
                        if (binPath != null)
                        {
                            return binPath;
                        }
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (SystemException) { } // Catch other exceptions such as IOException or SecurityException.

                return null;
            }
        }

        

        public static string Find(string pathDir, string file)
        {
            
           DirectoryInfo d;
            try
            {
                d = new DirectoryInfo(pathDir);
                DirectoryInfo[] w = d.GetDirectories(); //подкаталоги

                foreach (var item1 in w)
                {
                    //проверка на системную директорию, если системная пропускаем
                    if (item1.Attributes.Equals(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory | FileAttributes.ReparsePoint | FileAttributes.NotContentIndexed)) continue; //выходим из цикла
                    if (item1.Attributes.Equals(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory )) continue; //выходим из цикла
                                                                                                                                     //
                    string[] arrFile = Directory.EnumerateFiles(item1.FullName).ToArray();//получаем коллекцию всех файлов в директории
                    for (int n = 0; n < arrFile.Length; n++)
                    {
                        var ew = Path.GetFileName(arrFile[n]);
                        if (ew.Equals(file)) return arrFile[n];
                    }
                   
                }


                
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

       
        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Конвертируем строку пароля и соль в массив байтов
                byte[] bytes = Encoding.UTF8.GetBytes(password + salt);

                // Вычисляем хеш
                byte[] hash = sha256.ComputeHash(bytes);

                // Конвертируем хеш (массив байтов) обратно в строку, чтобы сохранить в базе данных
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }

}
