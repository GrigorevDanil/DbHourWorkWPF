using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DbHourWorkWPF.ViewModel
{
    class ConnectionVM : Utilities.ViewModelBase
    {
        string server, port, dbname, user, pass;
        public ObservableCollection<string> Users { get; set; }

        public string Server
        {
            get { return server; }
            set { server = value; OnPropertyChanged(nameof(Server));}
        }
        public string Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(nameof(Port)); }
        }

        public string DbName
        {
            get { return dbname; }
            set { dbname = value; OnPropertyChanged(nameof(DbName)); }
        }

        public string User
        {
            get { return user; }
            set { user = value; Password = ""; OnPropertyChanged(nameof(User)); }
        }

        public string Password
        {
            get { return pass; }
            set { pass = value; OnPropertyChanged(nameof(Password)); }
        }


        RelayCommand? editCommand;
        RelayCommand? resetCommand;

        private readonly PageModel _pageModel;

        public ConnectionVM()
        {
            _pageModel = new PageModel();

            try { App.serviceDb.openConnection(); }
            catch
            {
                Server = App.db.stringConnection.Server;
                Port = App.db.stringConnection.Port.ToString();
                DbName = App.db.stringConnection.Database;
                User = App.db.stringConnection.UserID.ToString();
                Password = App.db.stringConnection.Password;
                return;
            }

            Users = new ObservableCollection<string>();

           

            //Взятие всех пользователей на сервере
            using (MySqlCommand command = new MySqlCommand("SELECT User FROM mysql.user", App.serviceDb.getConnection()))
            {
                using (MySqlDataReader reader = command.ExecuteReader()) while (reader.Read()) Users.Add(reader.GetString(0));
            }

            App.serviceDb.closeConnection();



            Server = App.db.stringConnection.Server;
            Port = App.db.stringConnection.Port.ToString();
            DbName = App.db.stringConnection.Database;
            User = App.db.stringConnection.UserID.ToString();
            Password = App.db.stringConnection.Password;
        }

        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((o) =>
                  {
                      App.manager.WritePrivateString("main", "StringConnection", $"Server={Server};Port={Port};User ID={User};{(Password.Length != 0 ? $"Password= {Password}; " : " ")}Database={DbName};Allow Zero DateTime=True");
                      // Создаем новый процесс (новый экземпляр приложения)
                      ProcessStartInfo startInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName);

                      // Запускаем новый процесс
                      Process.Start(startInfo);

                      // Закрываем текущее приложение
                      Application.Current.Shutdown();
                  }));
            }
        }

        public RelayCommand ResetCommand
        {
            get
            {
                return resetCommand ??
                  (resetCommand = new RelayCommand((o) =>
                  {
                      App.manager.WritePrivateString("main", "StringConnection", $"Server=127.0.0.1;Port=3306;User ID=root;Database=dbhrtime;Allow Zero DateTime=True");

                      // Создаем новый процесс (новый экземпляр приложения)
                      ProcessStartInfo startInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName);

                      // Запускаем новый процесс
                      Process.Start(startInfo);

                      // Закрываем текущее приложение
                      Application.Current.Shutdown();
                  }));
            }
        }
    }
}
