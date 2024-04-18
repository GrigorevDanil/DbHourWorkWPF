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
        public List<ItemEmployee> Employees { get; private set; }   
        List<int> idDay = new List<int>();

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

            if (App.serviceDb.OperationSelect("SELECT * FROM card WHERE IdEmployee = @idEmp AND DateWork = @date", [Employees[comboBoxEmp.SelectedIndex].Id.ToString(), DateTime.Parse(comboBoxDay.SelectedItem.ToString().Substring(0)).ToString("yyyy-MM-dd")]).Rows.Count > 0)
            {
                MessageBox.Show("Дубликат записи!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Card.Employee.Id = Employees[comboBoxEmp.SelectedIndex].Id;
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


            Employees = App.serviceDb.LoadListFromServer($"SELECT IdEmployee, Surname, Name, Lastname FROM employee WHERE DateDismissal IS NULL", reader =>
            {
                return new ItemEmployee()
                {
                    Id = reader.GetInt32("IdEmployee"),
                    Surname = reader.GetString("Surname"),
                    Name = reader.GetString("Name"),
                    Lastname = reader["Lastname"] != DBNull.Value ? reader.GetString("Lastname") : null,
                };
            });

            
            DateTime startDate = DateTime.Parse(Days[0].Substring(0));
            DateTime endDate = DateTime.Parse(Days[Days.Count-1].Substring(0));

            for (int i=0; i < Employees.Count;i++)
            {
                Employees[i].IsSelected = (bool) App.serviceDb.OperationSelect( "SELECT CheckWorkEmployee(@idEmp, @start,@end)" , [Employees[i].Id.ToString(), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")]).Rows[0][0];
            }
           
            
            
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
            comboBoxEmp.ItemsSource = Employees;
            comboBoxEmp.SelectedItem = Employees.Where(item => item.Id == Card.Employee.Id).First();
        }

        public ContextCard(ItemCard card, List<string> days)
        {
            InitializeComponent();
            Card = card;
            Days = days;

            Employees = App.serviceDb.LoadListFromServer($"SELECT IdEmployee, Surname, Name, Lastname FROM employee WHERE DateDismissal IS NULL", reader =>
            {
                return new ItemEmployee()
                {
                    Id = reader.GetInt32("IdEmployee"),
                    Surname = reader.GetString("Surname"),
                    Name = reader.GetString("Name"),
                    Lastname = reader["Lastname"] != DBNull.Value ? reader.GetString("Lastname") : null,
                };
            });


            DateTime startDate = DateTime.Parse(Days[0].Substring(0));
            DateTime endDate = DateTime.Parse(Days[Days.Count - 1].Substring(0));

            for (int i = 0; i < Employees.Count; i++)
            {
                Employees[i].IsSelected = (bool)App.serviceDb.OperationSelect("SELECT CheckWorkEmployee(@idEmp, @start,@end)", [Employees[i].Id.ToString(), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")]).Rows[0][0];
            }

            App.serviceDb.LoadComboBox(ref comboBoxMark, ref idDay, "Select * From manualday");
            comboBoxDay.ItemsSource = Days;
            DataContext = Card;
            comboBoxEmp.ItemsSource = Employees;
        }
    }
}
