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
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : UserControl
    {
        public Employee()
        {
            InitializeComponent();
        }

        private void butAddEmp_Click(object sender, RoutedEventArgs e)
        {
            new ContextEmployee().ShowDialog();
        }

        private void checkBoxPost_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxPost.IsChecked == true)
            {
                comboBoxPost.IsEnabled = true;
                if (comboBoxPost.Items.Count != 0) comboBoxPost.SelectedIndex = 0;
            }
            else
            {
                comboBoxPost.IsEnabled = false;
                comboBoxPost.SelectedIndex = -1;
            }
        }
    }
}
