using DbHourWorkWPF.Items;
using DbHourWorkWPF.Properties;
using DBHourWorkWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
    }
}
