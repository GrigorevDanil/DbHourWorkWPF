using DbHourWorkWPF.ViewModel;
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

namespace DbHourWorkWPF.View
{
    public partial class MessageWindow : Window
    {
        ImageSource WarningImageSource = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/IconWarning.png", UriKind.RelativeOrAbsolute));
        ImageSource QuestionImageSource = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/IconQuestion.png", UriKind.RelativeOrAbsolute));
        ImageSource InformationImageSource = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "/ImageEmployee.png", UriKind.RelativeOrAbsolute));

        public MessageWindow(string headerText, string text, MessageBoxButton buts = MessageBoxButton.OKCancel, MessageBoxImage images = MessageBoxImage.Question)
        {
            InitializeComponent();
            Header.Text = headerText;
            Text.Text = text;
            switch (buts)
            {
                case MessageBoxButton.OK:
                    {
                        butOk.Visibility = Visibility.Collapsed;
                        gridBut.ColumnDefinitions.RemoveAt(0);  
                    }
                    break;
            }
            switch (images)
            {
                case MessageBoxImage.Information:
                    {
                        ImageMessage.Source = InformationImageSource;

                    }
                    break;
                case MessageBoxImage.Error:
                    {
                        ImageMessage.Source = WarningImageSource;
                        
                    }
                    break;
                case MessageBoxImage.Question:
                    {
                        ImageMessage.Source = QuestionImageSource;
                    }
                    break;
            }
            ImageMessage.Width = 100;
            ImageMessage.Height = 100;

        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
