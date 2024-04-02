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


            //Setting Time
            lDateTime.Content = $"Дата: {DateTime.Now:dd.MM.yyyy} Время: {DateTime.Now:HH:mm}";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();


            /*
            if (UserName != null) lUser.Content = UserName;
            else lUser.Content = UserLogin;

            if (IconUser == null) iconUser.ImageSource = IconUser;
            else IconUser = new BitmapImage(new Uri("ImageEmployee.png", UriKind.Relative));
            */
        }

        private void Timer_Tick(object sender, EventArgs e) => lDateTime.Content = $"Дата: {DateTime.Now:dd.MM.yyyy} Время: {DateTime.Now:HH:mm}";
        private void buttonLeave_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Closed(object sender, EventArgs e) => new LoginContext().Show();

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
