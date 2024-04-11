using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DbHourWorkWPF.View
{
    /// <summary>
    /// Логика взаимодействия для Day.xaml
    /// </summary>
    public partial class Day : UserControl
    {
        public Day()
        {
            InitializeComponent();
            if (App.Account.Role == "Пользователь")
            {
                butDel.Visibility = Visibility.Collapsed;
                butAdd.Visibility = Visibility.Collapsed;
                ColOp.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
        }
    }
}
