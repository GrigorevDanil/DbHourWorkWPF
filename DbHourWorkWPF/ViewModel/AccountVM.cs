using DbHourWorkWPF.Items;
using DbHourWorkWPF.Model;
using DbHourWorkWPF.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using MySqlConnector;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security.RightsManagement;

namespace DbHourWorkWPF.ViewModel
{
    class AccountVM : Utilities.ViewModelBase
    {
        public static ImageSource defaultImageSource = new BitmapImage(new Uri("ImageEmployee.png", UriKind.RelativeOrAbsolute));
        public static bool defaultImageFlag = true, changePassFlag = false;
        
        string cmdEdit = "UPDATE user SET Image = @img, Name = @nam, Surname = @surn, Login = @log, PasswordHash = @passHash, Salt = @_salt WHERE IdUser = @_idUser";

        private readonly PageModel _pageModel;

        ItemUser curAccount;

        public ItemUser CurAccount
        {
            get 
            {
                return curAccount;
            }
            set
            {
                curAccount = value;
                App.txtNick.Text = curAccount.Surname + " " + curAccount.Name;
                App.imgUser.ImageSource = curAccount.Image;
                OnPropertyChanged(nameof(CurAccount));
            }
        }
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;


        void UpdateAccount()
        {
            CurAccount = App.serviceDb.LoadRecordFromServer($"SELECT Image,Name,Surname,Login,Role,IdUser FROM user WHERE IdUser = {CurAccount.Id}", reader =>
            {
                ItemUser user = new ItemUser
                {
                    Name = reader.GetString(1),
                    Surname = reader.GetString(2),
                    Login = reader.GetString(3),
                    Role = reader.GetString(4),
                    Id = reader.GetInt32(5),
                };

                if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                {
                    user.Image = BytesToImageSource(reader, "Image");
                }
                else user.Image = defaultImageSource;

                return user;
            });

            App.Account = CurAccount;
            OnPropertyChanged(nameof(CurAccount));
        }


        public AccountVM()
        {
            _pageModel = new PageModel();
            CurAccount = App.Account;
        }


        string[] FillParam(ItemUser item)
        {
            string[] param = new string[7];
            param[1] = item.Name;
            param[2] = item.Surname;
            param[3] = item.Login;
            param[4] = item.PasswordHash;
            param[5] = item.Salt;

            return param;
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

        
        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((o) =>
                  {
                      ItemUser vm = new ItemUser
                      {
                          Image = CurAccount.Image,
                          Surname = CurAccount.Surname,
                          Name = CurAccount.Name,
                          Login = CurAccount.Login,
                          PasswordHash = CurAccount.PasswordHash,
                          Salt = CurAccount.Salt,
                          Id = CurAccount.Id
                      };
                      ContextAccount contextAccount = new ContextAccount(vm);


                      if (contextAccount.ShowDialog() == true)
                      {
                          string[] param = FillParam(contextAccount.Account);
                          param[6] = CurAccount.Id.ToString();

                          if (!defaultImageFlag) App.serviceDb.OperationOnRecord(cmdEdit, param, ImageSourceToBytes(new PngBitmapEncoder(), contextAccount.Account.Image));
                          else App.serviceDb.OperationOnRecord(cmdEdit, param);

                          if (changePassFlag)
                          {
                              string passHash = App.HashPassword(contextAccount.newPassword, vm.Salt);
                              App.serviceDb.OperationOnRecord("Update user SET PasswordHash = @pass WHERE IdUser = @id", [passHash, param[6]]);
                          }
                          changePassFlag = false;
                          UpdateAccount();
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
                          App.serviceDb.DeleteRecord(CurAccount.Id.ToString(), "DELETE FROM user WHERE user.IdUser = @id");

                          // Создаем новый процесс (новый экземпляр приложения)
                          ProcessStartInfo startInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName);

                          // Запускаем новый процесс
                          Process.Start(startInfo);

                          // Закрываем текущее приложение
                          Application.Current.Shutdown();
                      }
                  }));
            }
        }
        
    }
}
