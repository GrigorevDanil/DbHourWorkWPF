using DbHourWorkWPF.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class ContextCard : Window
    {
        public ItemCard Card { get;private set; }
        public List<string> Days { get; private set; }
        List<int>
            idEmp = new List<int>(),
            idDay = new List<int>();

        private void textBoxHour_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);

        private void butClick_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxEmp.SelectedIndex == -1 ||
                comboBoxMark.SelectedIndex == -1 ||
                comboBoxDay.SelectedIndex == -1 ||
                textBoxHour.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (App.serviceDb.OperationSelect("SELECT * FROM card WHERE IdEmployee = @idEmp AND DateWork = @date", [idEmp[comboBoxEmp.SelectedIndex].ToString(), DateTime.Parse(comboBoxDay.SelectedItem.ToString().Substring(0)).ToString("yyyy-MM-dd")]).Rows.Count > 0)
            {
                MessageBox.Show("Дубликат записи!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Card.Employee.Id = idEmp[comboBoxEmp.SelectedIndex];
            Card.WorkTimes.Add(new WorkTime());
            Card.WorkTimes[0].Day.Id = idDay[comboBoxMark.SelectedIndex];
            Card.WorkTimes[0].DateWork = comboBoxDay.SelectedItem.ToString();
            Card.WorkTimes[0].HourWork = int.Parse(textBoxHour.Text.ToString());

            


            DialogResult = true;
        }

        public ContextCard(ItemCard card, List<string> days, int indexDay, int indexWork = -1, bool flag = false)
        {
            InitializeComponent();
            Card = card;
            Days = days;
            App.serviceDb.LoadComboBox(ref comboBoxEmp, ref idEmp, "Select * From employee", 5, 3);
            App.serviceDb.LoadComboBox(ref comboBoxMark, ref idDay, "Select * From manualday");
            comboBoxDay.ItemsSource = Days;
            comboBoxEmp.SelectedItem = Card.Employee.Surname + " " + Card.Employee.Name + " " + Card.Employee.Lastname;
            comboBoxDay.SelectedItem = days[indexDay];
            if (flag)
            {
                headerText.Text = "Редактирование дня";
                comboBoxMark.SelectedItem = Card.WorkTimes[indexWork].Day.ShortName;
                textBoxHour.Text = Card.WorkTimes[indexWork].HourWork.ToString();
            }
            DataContext = Card;

        }

        public ContextCard(ItemCard card, List<string> days)
        {
            InitializeComponent();
            Card = card;
            Days = days;
            App.serviceDb.LoadComboBox(ref comboBoxEmp, ref idEmp, "Select * From employee", 5, 3);
            App.serviceDb.LoadComboBox(ref comboBoxMark, ref idDay, "Select * From manualday");
            comboBoxDay.ItemsSource = Days;
            DataContext = Card;

        }
    }
}
