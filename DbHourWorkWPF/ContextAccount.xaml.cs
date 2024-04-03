using DbHourWorkWPF.Items;
using DbHourWorkWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для ContextAccount.xaml
    /// </summary>
    public partial class ContextAccount : Window
    {
        public string curPassword { get; private set; }
        public string newPassword { get; private set; }
        public ItemUser Account { get; private set; }
        public ContextAccount()
        {
            InitializeComponent();
        }

        public ContextAccount(ItemUser acc)
        {
            InitializeComponent();
            Account = acc;
            
            if (App.Account.Image != AccountVM.defaultImageSource) AccountVM.defaultImageFlag = false;
            else AccountVM.defaultImageFlag = true;
            DataContext = Account;
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLogin.Text == "" ||
                textBoxSurname.Text == "" ||
                textBoxName.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }

        private void butChangeImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (dlg.ShowDialog() == true)
            {
                Account.Image = new BitmapImage(new Uri(dlg.FileName, UriKind.Relative));
                AccountVM.defaultImageFlag = false;
            }
        }

        private void butDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            Account.Image = AccountVM.defaultImageSource;
            AccountVM.defaultImageFlag = true;
        }

        private void butChangePass_Click(object sender, RoutedEventArgs e)
        {
            ContextChangeKey contextChangeKey = new ContextChangeKey(Account.Id);
            if (contextChangeKey.ShowDialog() == true)
            {
                AccountVM.changePassFlag = true;
                newPassword = contextChangeKey.textBoxNewKey.Password;
                curPassword = contextChangeKey.textBoxCurrentKey.Password;
            }
        }
    }
}
