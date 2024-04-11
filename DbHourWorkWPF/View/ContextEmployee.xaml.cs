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
    /// Логика взаимодействия для ContextEmployee.xaml
    /// </summary>
    public partial class ContextEmployee : Window
    {
        List<int> idPost = new List<int>();
        public ItemEmployee Employee { get; private set; }

        public ContextEmployee()
        {
            InitializeComponent();

        }

        public ContextEmployee(ItemEmployee emp,bool flag = false)
        {
            InitializeComponent();
            App.serviceDb.LoadComboBox(ref comboBoxPost, ref idPost, "Select * From manualpost");
            if (emp.Lastname == null) checkBoxLastname.IsChecked = false;
            if (emp.DateDismissal != null) checkBoxDateDis.IsChecked = true;
            Employee = emp;
            Employee.DateEmployment = DateTime.Now.ToString("yyyy-MM-dd");
            DataContext = Employee;
            if (flag) headerText.Text = "Редактирование сотрудника";
        }

        private void CheckBoxLastname_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (textBoxLastname == null) return;
            if (checkBoxLastname.IsChecked == true) textBoxLastname.IsEnabled = true;
            else textBoxLastname.IsEnabled = false;
        }
        private void CheckBoxDismissal_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (datePickerDis == null) return;
            if (checkBoxDateDis.IsChecked == true) datePickerDis.IsEnabled = true;
            else datePickerDis.IsEnabled = false;
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxPost.SelectedIndex == -1 ||
                textBoxNumEmp.Text == "" ||
                textBoxSurname.Text == "" ||
                textBoxName.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (checkBoxLastname.IsChecked == false) Employee.Lastname = null;
            if (checkBoxDateDis.IsChecked == false) Employee.DateDismissal = null;

            DialogResult = true;
        }

        private void comboBoxPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee.IdPost = idPost[comboBoxPost.SelectedIndex];
        }

        private void datePickerEmp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Employee != null) Employee.DateEmployment = datePickerEmp.SelectedDate.Value.ToString("yyyy-MM-dd");
        }

        private void datePickerDis_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee.DateDismissal = datePickerDis.SelectedDate.Value.ToString("yyyy-MM-dd");
        }
    }
}
