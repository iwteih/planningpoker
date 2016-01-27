using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PlanningPoker.Converter
{
    public class Bool2Visible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            bool v;
            if (bool.TryParse(value.ToString(), out v))
            {
                return v ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
