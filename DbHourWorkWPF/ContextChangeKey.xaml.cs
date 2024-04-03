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
    /// Логика взаимодействия для ContextChangeKey.xaml
    /// </summary>
    public partial class ContextChangeKey : Window
    {
        public int IdUser { get; private set; }
        public ContextChangeKey()
        {
            InitializeComponent();
        }
        public ContextChangeKey(int id)
        {
            InitializeComponent();
            IdUser = id;
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCurrentKey.Password.Length == 0 || textBoxNewKey.Password.Length == 0)
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var table = App.serviceDb.OperationSelect("Select PasswordHash,Salt FROM user WHERE IdUser = @id", [IdUser.ToString()]);
            string 
                curHash = table.Rows[0][0].ToString(),
                salt = table.Rows[0][1].ToString();

            if (curHash != App.HashPassword(textBoxCurrentKey.Password, salt))
            {
                MessageBox.Show("Текущий пароль задан неверно!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }
    }
}
