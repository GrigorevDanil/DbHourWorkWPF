using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DbHourWorkWPF.ViewModel
{
    class AdminVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public ObservableCollection<ItemUser> Users { get; set; }

        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;

        private string inputTextFilter, selectedRole;
        /*
        public string SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                if (selectedRole == null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost";
                else if (selectedRole == null && (inputTextFilter != null || inputTextFilter != "")) cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                else if (selectedRole != null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id}";
                else cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id} AND Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                UpdateListEmployee();
                OnPropertyChanged("SelectedPost");

            }
        }

        public string InputTextFilter
        {
            get
            {
                return inputTextFilter;
            }
            set
            {
                inputTextFilter = value;
                if (selectedPost == null && inputTextFilter == "") cmdEmp = "SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost";
                else if (selectedPost == null && inputTextFilter != "") cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                else cmdEmp = $"SELECT IdEmployee, employee.IdPost, manualpost.Title, NumEmployee, Surname,Name, Lastname , DateEmployment, DateDismissal FROM employee LEFT JOIN manualpost ON manualpost.IdPost = employee.IdPost WHERE manualpost.IdPost = {selectedPost.Id} AND Lower(CONCAT(NumEmployee,Surname,Name,Lastname)) LIKE '%{inputTextFilter}%'";
                UpdateListEmployee();
                OnPropertyChanged("InputTextFilter");
            }
        }
        */
        void UpdateListUsers()
        {
            Users = new ObservableCollection<ItemUser>(App.serviceDb.LoadListFromServer("Select * FROM user", reader =>
            {
                ItemUser user = new ItemUser
                {
                    Id = reader.GetInt32("Id"),
                    Login = reader.GetString("Login"),
                    Name = reader.GetString("Name"),
                    Surname = reader.GetString("Surname"),
                    Role = reader.GetString("Role"),
                    IsLock = reader.GetBoolean("IsLock"),
                    
                };

                if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                {
                    user.Image = BytesToImageSource(reader, "Image");
                }
                else user.Image = AccountVM.defaultImageSource;

                return user;
            }));
            OnPropertyChanged(nameof(Users));
        }

        public ImageSource BytesToImageSource(MySqlDataReader reader, string columnName)
        {
            int columnIndex = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(columnIndex))
            {
                long blobSize = reader.GetBytes(columnIndex, 0, null, 0, 0); // Получаем размер BLOB данных.
                byte[] buffer = new byte[blobSize];
                reader.GetBytes(columnIndex, 0, buffer, 0, (int)blobSize); // Считываем BLOB в буфер.

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    var image = new BitmapImage();
                    memoryStream.Position = 0; //Важно перейти к началу потока
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = memoryStream;
                    image.EndInit();
                    image.Freeze(); // Оптимально для использования в UI
                    return image;
                }
            }
            return null; // Или возвращайте изображение по умолчанию, если BLOB пустой.
        }

        public AdminVM()
        {
            _pageModel = new PageModel();
        }
    }
}
