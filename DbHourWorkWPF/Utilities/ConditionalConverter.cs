using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace DbHourWorkWPF.Utilities
{
    public class ConditionalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Предположим, что value - это boolean
            if (value is bool boolValue)
            {
                // Если условие истинно, возвращаем одно значение, иначе - другое
                
                return boolValue ? "Заблокирован" : "Разблокирован";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
