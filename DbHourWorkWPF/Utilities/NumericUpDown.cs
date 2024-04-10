using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;
using System.Windows.Controls;

namespace DbHourWorkWPF.Utilities
{
    public class NumericUpDown : TextBox
    {


        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(NumericUpDown), new PropertyMetadata(""));


        private Button upButton, downButton;


        public NumericUpDown()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("upButton", this) is Button _upButton)
            {
                upButton = _upButton;
                upButton.Click += UpButton_Click;
            }

            if (Template.FindName("downButton", this) is Button _downButton)
            {
                downButton = _downButton;
                downButton.Click += DownButton_Click;
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(Text, out int value))
                {
                    Text = $"{++value}";
                }
                else throw new Exception("Place enter a valid number");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(Text, out int value))
                {
                    Text = $"{--value}";
                }
                else throw new Exception("Place enter a valid number");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
