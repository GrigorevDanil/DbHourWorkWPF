using DbHourWorkWPF.Utilities;
using System.Windows.Input;


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

        public ICommand EmployeeCommand { get; set; }
        public ICommand TimeCommand { get; set; }
        public ICommand AccountCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand ConnectionCommand { get; set; }
        public ICommand DayCommand { get; set; }



        private void Employee(object obj) => CurrentView = new EmployeeVM();
        private void Time(object obj) => CurrentView = new TimeVM();
        private void Account(object obj) => CurrentView = new AccountVM();
        private void Copy(object obj) => CurrentView = new CopyVM();
        private void Connection(object obj) => CurrentView = new ConnectionVM();
        private void Day(object obj) => CurrentView = new DayVM();


        public NavigationVM()
        {
            EmployeeCommand = new RelayCommand(Employee);
            TimeCommand = new RelayCommand(Time);
            AccountCommand = new RelayCommand(Account);
            CopyCommand = new RelayCommand(Copy);
            ConnectionCommand = new RelayCommand(Connection);
            DayCommand = new RelayCommand(Day);

            // Startup Page
            CurrentView = new TimeVM();
        }
    }
}
