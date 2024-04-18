using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DbHourWorkWPF.Utilities
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Здесь ваше условие для изменения цвета
            if (value is bool val)
            {
                return val ? 
                    Brushes.Black : 
                    Brushes.Red;  // Красный цвет для подходящих условию элементов
            }
            return Brushes.Black;  // Обычный цвет для всех остальных
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
