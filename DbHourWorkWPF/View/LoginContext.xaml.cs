using DbHourWorkWPF.Items;
using DbHourWorkWPF.Properties;
using DbHourWorkWPF.Utilities;
using DBHourWorkWPF.Utilities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для LoginContext.xaml
    /// </summary>
    public partial class LoginContext : Window
    {

        List<string> listNameHost = new List<string>();
        public static string stringConnection;
        public static bool flagChangeUserServer;

        public LoginContext()
        {
            InitializeComponent();

            //Инициализация базы данных
            
            var connection = new MySqlConnection(App.manager.GetPrivateString("main", "StringConnection"));
            App.db = new Database(connection, new MySqlConnectionStringBuilder(connection.ConnectionString));
            App.serviceDb = new DatabaseService(App.db);

            //Проверка подключения
            try { App.serviceDb.openConnection(); }
            //База данных не найдена
            catch (MySqlException exp)
            {
                if (exp.Message == $"Unknown database \'{App.db.connection.Database}\'")
                {
                    if (MessageBox.Show("База данных не найдена? Создать базу данных по умолчанию?", "Отсутствует база данных!", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                    {
                        //Создание базы данных
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            RedirectStandardInput = true,
                            UseShellExecute = false
                        };

                        string pathXampp = App.manager.GetPrivateString("main", "PathXampp");

                        if (!Directory.Exists(pathXampp))
                        {
                            pathXampp = App.FindXamppMysqlBin();
                            App.manager.WritePrivateString("main", "PathXampp", pathXampp);
                        }

                        Process process = new Process { StartInfo = startInfo };

                        process.Start();
                        process.StandardInput.WriteLine($"SetBackup.bat {pathXampp} {Directory.GetCurrentDirectory()} DefaultBackup.sql");
                        process.WaitForExit();
                    }
                }
                else
                {
                    //Форма настройки базы данных
                    if (MessageBox.Show("Перейти к настройкам подключения с базой данных?", "Отсутствует соединение с сервером MySql!", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes) new DBForm().ShowDialog();
                }
            }
            finally { App.serviceDb.closeConnection(); }

            App.serviceDb.openConnection();

            //Взятие всех пользователей на сервере
            using (MySqlCommand command = new MySqlCommand("SELECT User, Host FROM mysql.user", App.serviceDb.getConnection()))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBoxLogin.Items.Add(reader.GetString(0));
                        listNameHost.Add(reader.GetString(1));
                    }
                }
            }

            App.serviceDb.closeConnection();
        }

        private async void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            //Текущий пользователь
            App.Account = new ItemUser();

            

            //Взятие пароля
            string curPass = buttonEye.IsChecked == true ? textBoxPass.Text : passwordBox.Password;

            App.serviceDb.openConnection();

            //Проверка на то что пользователь с сервера
            if (checkBoxServer.IsChecked == false)
            {
                if (textBoxLog.Text == "" && textBoxPass.Text == "" || passwordBox.Password == "")
                {
                    MessageBox.Show("Поле пустое!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (flagChangeUserServer)
                {
                    App.db.connection = new MySqlConnection("server=localhost; user = root; database = dbhrtime");
                    flagChangeUserServer = false;
                }

                //Вспомогательные таблицы
                DataTable table, tableCheckPass;

                //Попытка найти пользователя в таблице
                table = App.serviceDb.OperationSelect("SELECT * FROM user WHERE Login = @login;", [ textBoxLog.Text, curPass ]);


                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];

                    if ((bool)row["IsLock"] == true)
                    {
                        MessageBox.Show("Пользователь заблокирован. Обратитесь к администратору!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (row["DateLock"] != DBNull.Value)
                    {
                        DateTime tempTimeDB = (DateTime)row["DateLock"];
                        TimeSpan timeLeft = tempTimeDB.ToLocalTime().AddMinutes(1) - DateTime.Now;

                        if (tempTimeDB.AddMinutes(1) > DateTime.Now)
                        {
                            MessageBox.Show($"Пользователь будет разблокирован через {timeLeft.Seconds} сек", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); ;
                            return;
                        }
                        else
                        {
                            //Разблокировка пользователя и установка счетчика на 3 попытки
                            App.serviceDb.OperationOnRecord("UPDATE user SET DateLock = @date, CountAttemp = @count WHERE Login = @login;", [ null, "3", textBoxLog.Text]);
                            table = App.serviceDb.OperationSelect("SELECT * FROM user WHERE Login = @login;", [ textBoxLog.Text]);
                            row = table.Rows[0];
                        }
                    }

                    int tempCount = int.Parse(row["CountAttemp"].ToString());

                    //Получение иконки пользователя или установка обычной аватарки
                    BitmapImage bitmapImage = new BitmapImage();
                    if (row["Image"] != DBNull.Value)
                    {
                        using (MemoryStream ms = new MemoryStream((byte[])row["Image"]))
                        {
                            ms.Position = 0;

                            bitmapImage.BeginInit();
                            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.UriSource = null;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();
                        }
                        bitmapImage.Freeze();
                        App.Account.Image = bitmapImage;
                    }
                    else App.Account.Image = new BitmapImage(new Uri("ImageEmployee.png", UriKind.Relative));


                    App.Account.Id = int.Parse(row["IdUser"].ToString());
                    App.Account.Login = row["Login"].ToString();
                    App.Account.Role = row["Role"].ToString();
                    App.Account.Surname = row["Surname"].ToString();
                    App.Account.Name = row["Name"].ToString();
                    if (row["DateLock"].ToString() != "") App.Account.DateLock = DateTime.Parse(row["DateLock"].ToString()).ToString("dd.MM.yyyy");
                    App.Account.IsLock = bool.Parse(row["IsLock"].ToString());
                    App.Account.PasswordHash = row["PasswordHash"].ToString();
                    App.Account.Salt = row["Salt"].ToString();

                    //Проверка логина и пароля
                    
                    string tempPassHash = App.HashPassword(curPass, App.Account.Salt);

                    if (App.Account.PasswordHash != tempPassHash)
                    {
                        if (tempCount == 0)
                        {
                            //Блокировка пользователя
                            App.serviceDb.OperationOnRecord("UPDATE user SET DateLock = @date WHERE IdUser = @idUser;", [  DateTime.Now.ToString("yyyy-MM-dd"), App.Account.Id.ToString() ]);
                            App.serviceDb.openConnection();
                            MessageBox.Show($"Пользователь {textBoxLog.Text} был заблокирован на 1 минуту. Повторите попытку позже ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        //Уменьшение счетчика на 1 при неправильном пароле
                        App.serviceDb.OperationOnRecord("UPDATE user SET CountAttemp = CountAttemp-1 WHERE IdUser = @idUser;", [ App.Account.Id.ToString() ]);
                        App.serviceDb.openConnection();
                        MessageBox.Show($"Неверный пароль! Осталось попыток входа: {tempCount}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    buttonEnter.Content = "Успешно!";
                    await Task.Delay(500);
                    MainForm form = new MainForm();
                    form.Show();
                    textBoxLog.Text = "";
                    textBoxPass.Text = "";
                    buttonEnter.Content = "Войти";
                    this.Hide();
                }
                else
                {
                    buttonEnter.Content = "Пользователь не найден!";
                    await Task.Delay(1000);
                    buttonEnter.Content = "Вход";
                }

            }
            else
            {
                flagChangeUserServer = true;
                if (comboBoxLogin.SelectedIndex == -1)
                {
                    MessageBox.Show("Пользователь не выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                stringConnection = $"server={textBoxHost.Text}; user = {comboBoxLogin.SelectedItem}; password = {curPass}; database = dbhrtime";
                App.db.connection = new MySqlConnection(stringConnection);
                try { App.serviceDb.openConnection(); }
                catch (Exception ex)
                {
                    App.db.connection = new MySqlConnection("server=localhost; user = root; database = dbhrtime");
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                finally
                {
                    App.serviceDb.closeConnection();
                }
                App.Account.Login = comboBoxLogin.SelectedItem.ToString();
                App.Account.Image = new BitmapImage(new Uri("ImageEmployee.png", UriKind.Relative));
                buttonEnter.Content = "Успешно!";
                await Task.Delay(500);
                MainForm form = new MainForm();
                form.Show();
                textBoxLog.Text = "";
                textBoxPass.Text = "";
                buttonEnter.Content = "Вход";
                this.Hide();
            }

        }

        private void checkBoxServer_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxServer.IsChecked == true)
            {
                //Анимация смены полей
                AnimateElementDownUp(textBoxLog, 10, TimeSpan.FromSeconds(0.5), true);
                AnimateElementDownUp(textBoxHost, -10, TimeSpan.FromSeconds(0.5), false);
                AnimateElementDownUp(comboBoxLogin, -10, TimeSpan.FromSeconds(0.5), false);
            }
            else
            {
                //Анимация смены полей
                AnimateElementDownUp(textBoxLog, -10, TimeSpan.FromSeconds(0.5), false);
                AnimateElementDownUp(textBoxHost, 10, TimeSpan.FromSeconds(0.5), true);
                AnimateElementDownUp(comboBoxLogin, 10, TimeSpan.FromSeconds(0.5), true);
            }
        }




        private void AnimateElementDownUp(UIElement element, double offset, TimeSpan duration, bool flagHide)
        {
            // Убедитесь, что у элемента есть Transform, который можно анимировать
            if (element.RenderTransform == null ||
                !(element.RenderTransform is TranslateTransform))
            {
                element.RenderTransform = new TranslateTransform();
            }

            // Создать Storyboard
            var storyboard = new Storyboard();

            // Анимация перемещения вниз или вверх
            var moveDownAnimation = new DoubleAnimation()
            {
                From = 0,
                To = offset,
                Duration = duration,
                AccelerationRatio = 0.2, // Начать медленно
                DecelerationRatio = 0.8 // Закончить быстро
            };
            Storyboard.SetTarget(moveDownAnimation, element);
            Storyboard.SetTargetProperty(moveDownAnimation, new PropertyPath("RenderTransform.Y"));

            // Добавить анимации в Storyboard
            storyboard.Children.Add(moveDownAnimation);
            if (flagHide) storyboard.Children.Add(AnimVisInvis(1.0f, 0.0f, Visibility.Hidden));
            else storyboard.Children.Add(AnimVisInvis(0.0f, 1.0f, Visibility.Visible));


            // Запустить анимацию
            storyboard.Begin();

            DoubleAnimation AnimVisInvis(float from, float to, Visibility stat)
            {
                // Анимация исчезновения и появления
                var fadeAnimation = new DoubleAnimation()
                {
                    From = from,
                    To = to,
                    Duration = duration
                };
                Storyboard.SetTarget(fadeAnimation, element);
                Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(UIElement.OpacityProperty));
                element.Visibility = stat;
                return fadeAnimation;
            }
        }

        private void buttonEye_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (buttonEye.IsChecked == true)
            {
                imageEye.Source = new BitmapImage(new Uri("/Pictures/Visible.png", UriKind.Relative));
                passwordBox.Visibility = Visibility.Hidden;
                textBoxPass.Visibility = Visibility.Visible;
                textBoxPass.Text = passwordBox.Password;
            }
            else
            {
                imageEye.Source = new BitmapImage(new Uri("/Pictures/inVisible.png", UriKind.Relative));
                passwordBox.Visibility = Visibility.Visible;
                if (passwordBox.Password.Length == 0) textBoxPass.Visibility = Visibility.Visible;
                else textBoxPass.Visibility = Visibility.Hidden;
                passwordBox.Password = textBoxPass.Text;
            }
            imageEye.Height = 33;
            imageEye.Width = 33;
        }

        private void textBoxPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (buttonEye.IsChecked == false)
            {
                textBoxPass.Visibility = Visibility.Hidden;
                passwordBox.Focus();
                return;
            }

        }

        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Length == 0)
            {
                textBoxPass.Visibility = Visibility.Visible;
                textBoxPass.Text = "";
                passwordBox.Password = "";
            }
        }

        private void comboBoxLogin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxLogin.SelectedIndex == -1) return;
            if (listNameHost[comboBoxLogin.SelectedIndex] == "%") textBoxHost.Text = "localhost";
            else textBoxHost.Text = listNameHost[comboBoxLogin.SelectedIndex];
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
