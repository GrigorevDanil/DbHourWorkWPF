﻿using DbHourWorkWPF.Items;
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

namespace DbHourWorkWPF
{
    /// <summary>
    /// Логика взаимодействия для ContextPost.xaml
    /// </summary>
    public partial class ContextPost : Window
    {
        public ItemPost Post { get; private set; }
        public ContextPost()
        {
            InitializeComponent();
        }

        public ContextPost(ItemPost post, bool flag = false)
        {
            InitializeComponent();
            Post = post;
            DataContext = Post;
            if (flag) headerText.Text = "Редактирование должности";
        }

        private void butEnter_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxTitle.Text == "")
            {
                MessageBox.Show("Поля не заполнены!", "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }
    }
}
