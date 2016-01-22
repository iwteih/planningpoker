using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PlanningPoker.Converter
{
    class FaceVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            String v = value.ToString().ToLower();

            foreach(string e in Enum.GetNames(typeof(CardStatus)))
            {
                if(e.ToLower() == v)
                {
                    return Visibility.Hidden;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
