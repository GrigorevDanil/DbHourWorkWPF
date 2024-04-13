using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        
        private void butResizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void butHideWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public MainForm()
        {
            InitializeComponent();

            App.txtNick = tbNick;
            App.imgUser = iconUser;
            //Setting Time
            lDateTime.Content = $"Дата: {DateTime.Now:dd.MM.yyyy} Время: {DateTime.Now:HH:mm}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            if (LoginContext.flagChangeUserServer)
            {
                tbNick.Text = App.Account.Login;
                butAccount.Visibility = Visibility.Collapsed;
                butCopy.Visibility = Visibility.Collapsed;
                butConnect.Visibility = Visibility.Collapsed;
                menuItemAdmin.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbNick.Text = App.Account.Surname + " " + App.Account.Name;
                if (App.Account.Role == "Кадровик" || App.Account.Role == "Пользователь")
                {
                    butCopy.Visibility = Visibility.Collapsed;
                    butConnect.Visibility = Visibility.Collapsed;
                    menuItemAdmin.Visibility = Visibility.Collapsed;
                }

            }

        }

        private void Timer_Tick(object sender, EventArgs e) => lDateTime.Content = $"Дата: {DateTime.Now:dd.MM.yyyy} Время: {DateTime.Now:HH:mm}";
        private void buttonLeave_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Closed(object sender, EventArgs e) => new LoginContext().Show();

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void butAdmin_Click(object sender, RoutedEventArgs e)
        {
            foreach(RadioButton but in panelButton.Children) but.IsChecked = false;
        }

    }
}
