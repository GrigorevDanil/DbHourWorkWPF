using DbHourWorkWPF.Items;
using DbHourWorkWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для ContextAccount.xaml
    /// </summary>
    public partial class ContextAccount : Window
    {
        public string curPassword { get; private set; }
        public string newPassword { get; private set; }
        public ItemUser Account { get; private set; }
        public ContextAccount()
        {
            InitializeComponent();
        }

        public ContextAccount(ItemUser acc)
        {
            InitializeComponent();
            Account = acc;
            
            if (App.Account.Image != AccountVM.defaultImageSource) AccountVM.defaultImageFlag = false;
            else AccountVM.defaultImageFlag = true;
            DataContext = Account;
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLogin.Text == "" ||
                textBoxSurname.Text == "" ||
                textBoxName.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }

        private void butChangeImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                Account.Image = ReduceBitmapImageSize(new BitmapImage(new Uri(dlg.FileName, UriKind.Relative)));
                AccountVM.defaultImageFlag = false;
            }
        }

        private void butDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            Account.Image = AccountVM.defaultImageSource;
            AccountVM.defaultImageFlag = true;
        }

        private void butChangePass_Click(object sender, RoutedEventArgs e)
        {
            ContextChangeKey contextChangeKey = new ContextChangeKey(Account.Id);
            if (contextChangeKey.ShowDialog() == true)
            {
                AccountVM.changePassFlag = true;
                newPassword = contextChangeKey.textBoxNewKey.Password;
                curPassword = contextChangeKey.textBoxCurrentKey.Password;
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
    }
}
