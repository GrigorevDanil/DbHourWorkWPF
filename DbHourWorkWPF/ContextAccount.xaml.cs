using DbHourWorkWPF.Items;
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
    /// Логика взаимодействия для ContextAccount.xaml
    /// </summary>
    public partial class ContextAccount : Window
    {
        public ItemUser Account { get; private set; }
        public ContextAccount()
        {
            InitializeComponent();
        }

        public ContextAccount(ItemUser acc)
        {
            InitializeComponent();
            Account = acc;
            DataContext = Account;
        }
    }
}
