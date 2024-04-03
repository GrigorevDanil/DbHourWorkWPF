using DbHourWorkWPF.Items;
using DbHourWorkWPF.Properties;
using DBHourWorkWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public static Setting settingJson;
        public static DatabaseService serviceDb;
        public static ItemUser Account;
        public static TextBlock txtNick;
        public static ImageBrush imgUser;

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
