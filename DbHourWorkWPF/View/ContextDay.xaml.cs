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
    /// Логика взаимодействия для ContextDay.xaml
    /// </summary>
    public partial class ContextDay : Window
    {
        public ItemDay Day { get; private set; }
        public ContextDay()
        {
            InitializeComponent();
        }

        public ContextDay(ItemDay day)
        {
            InitializeComponent();
            Day = day;
            DataContext = Day;
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxShortName.Text == "" ||
                textBoxTitle.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }
    }
}
