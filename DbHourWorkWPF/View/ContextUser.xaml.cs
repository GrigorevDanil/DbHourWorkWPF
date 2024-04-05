using DbHourWorkWPF.Items;
using DbHourWorkWPF.View;
using DbHourWorkWPF.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для ContextUser.xaml
    /// </summary>
    public partial class ContextUser : Window
    {
        public ItemUser User { get; private set; }
        public string newPassword { get; private set; }

        bool flagAdd;
        public ContextUser()
        {
            InitializeComponent();
        }

        public ContextUser(ItemUser user, bool flagAdd = false)
        {
            InitializeComponent();
            comboBoxRole.Items.Add("Администратор");
            comboBoxRole.Items.Add("Кадровик");
            comboBoxRole.Items.Add("Пользователь");
            User = user;
            User.DateLock = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DataContext = User;
            if (User.Image != null) AdminVM.defaultImageFlag = false;
            this.flagAdd = flagAdd;
            if(flagAdd)
            {
                butChangePass.Visibility= Visibility.Hidden;
                textResetPass.Text = "Установить пароль";
                textResetPass.Foreground = new SolidColorBrush(Color.FromRgb(0xD9, 0xD9, 0xD9));
            }
        }

        private void CheckBoxLock_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (checkBoxDateLock.IsChecked == true) datePickerLock.IsEnabled = true;
            else datePickerLock.IsEnabled = false;
        }

        private void datePickerLock_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User != null) User.DateLock = datePickerLock.SelectedDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLogin.Text == "" ||
                textBoxSurname.Text == "" ||
                textBoxName.Text == "" ||
                comboBoxRole.SelectedIndex == -1)
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (flagAdd && !AdminVM.resetPassFlag)
            {
                butResetPass_Click(sender, e);
                if (!AdminVM.resetPassFlag) return;
            }
            

            if (checkBoxDateLock.IsChecked == false) User.DateLock = null;

            DialogResult = true;
        }

        private void butChangeImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (dlg.ShowDialog() == true)
            {
                User.Image = ReduceBitmapImageSize(new BitmapImage(new Uri(dlg.FileName, UriKind.Relative)));
                AdminVM.defaultImageFlag = false;
            }
        }

        private void butDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            User.Image = AdminVM.defaultImageSource;
            AdminVM.defaultImageFlag = true;
        }

        private void butChangePass_Click(object sender, RoutedEventArgs e)
        {
            ContextChangeKey contextChangeKey = new ContextChangeKey(User.Id);
            if (contextChangeKey.ShowDialog() == true)
            {
                AdminVM.changePassFlag = true;
                newPassword = contextChangeKey.textBoxNewKey.Password;
            }
        }

        private void butResetPass_Click(object sender, RoutedEventArgs e)
        {
            ContextResetKey contextResetKey = new ContextResetKey();
            if (contextResetKey.ShowDialog() == true)
            {
                AdminVM.resetPassFlag = true;
                newPassword = contextResetKey.textBoxNewKey.Password;
            }
        }

        public static BitmapImage ReduceBitmapImageSize(BitmapImage originalImage)
        {
            const long targetSizeKilobytes = 64;
            const long targetSize = targetSizeKilobytes * 1024;

            MemoryStream memoryStream = new MemoryStream();
            BitmapImage resizedImage = new BitmapImage();

            // Сначала попробуем изменить размер изображения
            for (double scale = 1.0; scale > 0.1; scale -= 0.1)
            {
                memoryStream.SetLength(0); // Сбросить memoryStream

                // Масштабируем изображение
                TransformedBitmap scaledBitmap = new TransformedBitmap(
                    originalImage,
                    new ScaleTransform(scale, scale)
                );

                // Проверяем, требуется ли нам сохранять в формате PNG
                if (originalImage.Format == PixelFormats.Bgra32 ||
                    originalImage.Format == PixelFormats.Pbgra32 ||
                    originalImage.Format == PixelFormats.Bgr32)
                {
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(scaledBitmap));
                    pngEncoder.Save(memoryStream);
                }
                else // Иначе используем JPEG
                {
                    JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder { QualityLevel = 50 }; // Выбираем среднее значение качества
                    jpegEncoder.Frames.Add(BitmapFrame.Create(scaledBitmap));
                    jpegEncoder.Save(memoryStream);
                }

                // Если размер файла удовлетворяет требованиям, загружаем его как BitmapImage
                if (memoryStream.Length <= targetSize)
                {
                    resizedImage.BeginInit();
                    resizedImage.StreamSource = new MemoryStream(memoryStream.ToArray()); // Делаем копию потока
                    resizedImage.CacheOption = BitmapCacheOption.OnLoad;
                    resizedImage.EndInit();
                    resizedImage.Freeze(); // Замораживаем для использования в других потоках
                    return resizedImage;
                }
            }

            throw new InvalidOperationException("Невозможно уменьшить размер изображения до желаемого размера.");
        }

        private void comboBoxRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User.Role = comboBoxRole.SelectedItem.ToString();
        }

        
    }
}
