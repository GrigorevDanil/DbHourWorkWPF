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

namespace DbHourWorkWPF.View
{
    /// <summary>
    /// Логика взаимодействия для ContextPrintCard.xaml
    /// </summary>
    public partial class ContextPrintCard : Window
    {
        public ContextPrintCard()
        {
            InitializeComponent();
            comboBoxMoth.Items.Add("Январь");
            comboBoxMoth.Items.Add("Февраль");
            comboBoxMoth.Items.Add("Март");
            comboBoxMoth.Items.Add("Апрель");
            comboBoxMoth.Items.Add("Май");
            comboBoxMoth.Items.Add("Июнь");
            comboBoxMoth.Items.Add("Июль");
            comboBoxMoth.Items.Add("Август");
            comboBoxMoth.Items.Add("Сентябрь");
            comboBoxMoth.Items.Add("Октябрь");
            comboBoxMoth.Items.Add("Ноябрь");
            comboBoxMoth.Items.Add("Декабрь");
            comboBoxMoth.SelectedIndex = 0;
            comboBoxEmp.SelectedIndex = -1;
        }

        private void checkBoxEmp_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxEmp.IsChecked == true) comboBoxEmp.SelectedIndex = 0;
            else comboBoxEmp.SelectedIndex = -1;
            comboBoxEmp.IsEnabled = (bool)checkBoxEmp.IsChecked;
        }


        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
