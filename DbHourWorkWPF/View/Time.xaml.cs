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
        }

        private void dataGridCard_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = sender as DataGrid;

            try
            {
                var currentColumnIndex = grid.CurrentCell.Column.DisplayIndex;
                ((TimeVM)DataContext).SelectedCellIndex = currentColumnIndex;
            }
            catch { }
        }
    }
}
