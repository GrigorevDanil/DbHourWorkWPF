using DbHourWorkWPF.ViewModel;
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
    /// Логика взаимодействия для Time.xaml
    /// </summary>
    public partial class Time : UserControl
    {
        public Time()
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

            if (App.Account.Role == "Пользователь")
            {
                butAdd.Visibility = Visibility.Collapsed;
                menu.Visibility = Visibility.Collapsed;
            }
        }

        private void dataGridCard_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid.CurrentCell.Column == null) return;
            ((TimeVM)DataContext).SelectedCellIndex = grid.CurrentCell.Column.DisplayIndex;
        }

        private void dataGridCard_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            var hitTestInfo = VisualTreeHelper.HitTest(grid, e.GetPosition(grid));
            var cell = FindVisualParent<DataGridCell>(hitTestInfo.VisualHit);

            if (cell != null)
            {
                // Выбираем ячейку
                grid.CurrentCell = new DataGridCellInfo(cell);

                // Переход в обработчик изменения выбранной ячейки
                dataGridCard_CurrentCellChanged(grid, EventArgs.Empty);
            }
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Поиск родительского объекта определенного типа
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        private void checkBoxEmp_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxEmp.IsChecked == true) comboBoxEmp.SelectedIndex = 0;
            else comboBoxEmp.SelectedIndex = -1;
            comboBoxEmp.IsEnabled = (bool)checkBoxEmp.IsChecked;
        }

    }
}
