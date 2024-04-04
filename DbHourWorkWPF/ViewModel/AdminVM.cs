using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using DbHourWorkWPF.View;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DbHourWorkWPF.ViewModel
{
    class AdminVM : Utilities.ViewModelBase
    {
        public static ImageSource defaultImageSource = new BitmapImage(new Uri("ImageEmployee.png", UriKind.RelativeOrAbsolute));
        private readonly PageModel _pageModel;

        string cmdEmp = "SELECT * FROM user",
          cmdAdd = "INSERT INTO user VALUES (NULL,@img,@_name,@_surn,@log, @passHash, @_salt, @_role, 3, @_lock,@date);",
          cmdEdit = "UPDATE user SET Image = @img, Name = @_name, Surname = @_surn, Login = @log,PasswordHash = @passHash,Salt = @_salt, Role = @_role, IsLock = @lock, DateLock = @date WHERE IdUser = @id";
        public ObservableCollection<ItemUser> Users { get; set; }

        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;
        RelayCommand? multiplydeleteCommand;

        private string inputTextFilter;
        ComboBoxItem selectedRole;
        public static bool defaultImageFlag = true, changePassFlag, resetPassFlag;

        public ComboBoxItem SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                if (selectedRole == null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = "SELECT * FROM user";
                else if (selectedRole == null && (inputTextFilter != null || inputTextFilter != "")) cmdEmp = $"SELECT * FROM user WHERE Lower(CONCAT(Login,Surname,Name)) LIKE '%{inputTextFilter}%'";
                else if (selectedRole != null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = $"SELECT * FROM user WHERE Role = '{selectedRole.Content}'";
                else cmdEmp = $"SELECT * FROM user WHERE Role = '{selectedRole.Content}' AND Lower(CONCAT(Login,Surname,Name)) LIKE '%{inputTextFilter}%'";
                UpdateListUsers();
                OnPropertyChanged(nameof(SelectedRole));

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
                if (selectedRole == null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = "SELECT * FROM user";
                else if (selectedRole == null && (inputTextFilter != null || inputTextFilter != "")) cmdEmp = $"SELECT * FROM user WHERE Lower(CONCAT(Login,Surname,Name)) LIKE '%{inputTextFilter}%'";
                else if (selectedRole != null && (inputTextFilter == null || inputTextFilter == "")) cmdEmp = $"SELECT * FROM user WHERE Role = '{selectedRole.Content}'";
                else cmdEmp = $"SELECT * FROM user WHERE Role = '{selectedRole.Content}' AND Lower(CONCAT(Login,Surname,Name)) LIKE '%{inputTextFilter}%'";
                UpdateListUsers();
                OnPropertyChanged("InputTextFilter");
            }
        }


        void UpdateListUsers()
        {
            Users = new ObservableCollection<ItemUser>(App.serviceDb.LoadListFromServer(cmdEmp, reader =>
            {
                ItemUser user = new ItemUser
                {
                    Id = reader.GetInt32("IdUser"),
                    Login = reader.GetString("Login"),
                    Name = reader.GetString("Name"),
                    Surname = reader.GetString("Surname"),
                    Role = reader.GetString("Role"),
                    IsLock = reader.GetBoolean("IsLock"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    Salt = reader.GetString("Salt"),
                    DateLock = reader["DateLock"] != DBNull.Value ? reader.GetDateTime("DateLock").ToString("dd.MM.yyyy HH:mm") : null
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

        public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;
            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }
            return bytes;
        }

        public AdminVM()
        {
            _pageModel = new PageModel();
            UpdateListUsers();
        }

        string[] FillParam(ItemUser item)
        {
            string[] param = new string[10];
            param[1] = item.Name;
            param[2] = item.Surname;
            param[3] = item.Login;
            param[4] = item.PasswordHash;
            param[5] = item.Salt;
            param[6] = item.Role;
            param[7] = item.IsLock ? "1" : "0";
            param[8] = item.DateLock != null ? DateTime.Parse(item.DateLock).ToString("yyyy-MM-dd HH:mm:ss") : null;

            return param;
        }

        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      ItemUser user = new ItemUser();
                      user.Image = defaultImageSource;
                      ContextUser contextUser = new ContextUser(user, true);
                      if (contextUser.ShowDialog() == true)
                      {
                          string salt = App.CreateSalt(), passHash = App.HashPassword(contextUser.newPassword, salt);
                          contextUser.User.PasswordHash = passHash;
                          contextUser.User.Salt = salt;
                          if (!defaultImageFlag) App.serviceDb.OperationOnRecord(cmdAdd, FillParam(contextUser.User), ImageSourceToBytes(new PngBitmapEncoder(), contextUser.User.Image));
                          else App.serviceDb.OperationOnRecord(cmdAdd, FillParam(contextUser.User));
                          resetPassFlag = false;
                          UpdateListUsers();
                      }
                  }));
            }
        }

        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      ItemUser? user = selectedItem as ItemUser;
                      if (user == null) return;

                      ItemUser vm = new ItemUser
                      {
                          Image = user.Image,
                          Surname = user.Surname,
                          Name = user.Name,
                          Login = user.Login,
                          PasswordHash = user.PasswordHash,
                          Salt = user.Salt,
                          Id = user.Id,
                          IsLock = user.IsLock,
                          DateLock = user.DateLock,
                          Role = user.Role
                      };
                      ContextUser contextUser = new ContextUser(vm);


                      if (contextUser.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextUser.User);
                          param[9] = vm.Id.ToString();

                          if (!defaultImageFlag) App.serviceDb.OperationOnRecord(cmdEdit, param, ImageSourceToBytes(new PngBitmapEncoder(), contextUser.User.Image));
                          else App.serviceDb.OperationOnRecord(cmdEdit, param);

                          if (changePassFlag)
                          {
                              string passHash = App.HashPassword(contextUser.newPassword, user.Salt);
                              App.serviceDb.OperationOnRecord("Update user SET PasswordHash = @pass WHERE IdUser = @id", [passHash, param[9]]);
                          }
                          if (resetPassFlag)
                          {
                              string salt = App.CreateSalt(), passHash = App.HashPassword(contextUser.newPassword, salt);
                              App.serviceDb.OperationOnRecord("Update user SET PasswordHash = @pass, Salt = @_salt WHERE IdUser = @id", [passHash,salt, param[9]]);
                          }
                          changePassFlag = false;
                          resetPassFlag = false;
                          UpdateListUsers();
                      }
                  }));
            }
        }


        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      if (MessageBox.Show("Вы уверены что хотите удалить данную запись?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                      {
                          ItemUser? user = selectedItem as ItemUser;
                          if (user == null) return;
                          App.serviceDb.DeleteRecord(user.Id.ToString(), "DELETE FROM user WHERE user.IdUser = @id");
                          UpdateListUsers();
                      }
                  }));
            }
        }

        // команда множественного удаления
        public RelayCommand MultiplyDeleteCommand
        {
            get
            {
                return multiplydeleteCommand ??
                  (multiplydeleteCommand = new RelayCommand((obj) =>
                  {
                      if (MessageBox.Show("Вы уверены что хотите удалить выбранные записи?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                      {
                          foreach (var user in Users.Where(u => u.IsSelected).ToList()) App.serviceDb.DeleteRecord(user.Id.ToString(), "DELETE FROM user WHERE user.IdUser = @id");
                          UpdateListUsers();
                      }
                  }));
            }
        }
    }
}
