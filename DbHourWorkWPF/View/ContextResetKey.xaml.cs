﻿using System;
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

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для ContextResetKey.xaml
    /// </summary>
    public partial class ContextResetKey : Window
    {
        public ContextResetKey()
        {
            InitializeComponent();
            
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxNewKey.Password.Length == 0)
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }
    }
}
