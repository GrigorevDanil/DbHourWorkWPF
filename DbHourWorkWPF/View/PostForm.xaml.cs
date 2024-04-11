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
using System.Windows.Shapes;

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для PostForm.xaml
    /// </summary>
    public partial class PostForm : Window
    {
        public PostForm()
        {
            InitializeComponent();
            if (App.Account.Role == "Пользователь")
            {
                butAddPost.Visibility = Visibility.Collapsed;
                ColOp.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
        }

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
