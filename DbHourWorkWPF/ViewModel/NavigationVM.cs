using DbHourWorkWPF.Items;
using DbHourWorkWPF.Utilities;
using MySqlConnector;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System;
using HtmlAgilityPack;
using DbHourWorkWPF.View;


namespace DbHourWorkWPF.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public RelayCommand? outputAuthorCommand { get; set; }
        public RelayCommand? printHtmlCommand { get; set; }
        public RelayCommand? helpCommand { get; set; }

        public ICommand EmployeeCommand { get; set; }
        public ICommand TimeCommand { get; set; }
        public ICommand AccountCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand ConnectionCommand { get; set; }
        public ICommand DayCommand { get; set; }
        public ICommand AdminCommand { get; set; }



        private void Employee(object obj) =>CurrentView = new EmployeeVM();
        private void Time(object obj) => CurrentView = new TimeVM();
        private void Account(object obj) => CurrentView = new AccountVM();
        private void Copy(object obj) => CurrentView = new CopyVM();
        private void Connection(object obj) => CurrentView = new ConnectionVM();
        private void Day(object obj) => CurrentView = new DayVM();
        private void Admin(object obj) => CurrentView = new AdminVM();


        public NavigationVM()
        {
            EmployeeCommand = new RelayCommand(Employee);
            TimeCommand = new RelayCommand(Time);
            AccountCommand = new RelayCommand(Account);
            CopyCommand = new RelayCommand(Copy);
            ConnectionCommand = new RelayCommand(Connection);
            DayCommand = new RelayCommand(Day);
            AdminCommand = new RelayCommand(Admin);

            // Startup Page
            CurrentView = new TimeVM();
        }

        // команда окна автора
        public RelayCommand OutputAuthorCommand
        {
            get
            {
                return outputAuthorCommand ??
                  (outputAuthorCommand = new RelayCommand((o) =>
                  {
                      new View.MessageWindow("Об авторе", "Программа была разработана студентом группы ИП-3 Григорьевым Д.В.", MessageBoxButton.OK, MessageBoxImage.Information).ShowDialog();
                  }));
            }
        }

        // команда печати табеля html
        public RelayCommand PrintHtmlCommand
        {
            get
            {
                return printHtmlCommand ??
                  (printHtmlCommand = new RelayCommand((o) =>
                  {
                      new ContextPrintCard().ShowDialog();
                  }));
            }
        }

        // команда открытия справки html
        public RelayCommand HelpCommand
        {
            get
            {
                return helpCommand ??
                  (helpCommand = new RelayCommand((o) =>
                  {
                      Process.Start(Directory.GetCurrentDirectory() + "/Help/help5.chm");
                  }));
            }
        }

    }
}
