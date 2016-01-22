using System;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanningPoker.Converter
{
    public class FaceBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }

            String v = value.ToString();
            int parseValue;
            bool isNumber = int.TryParse(v, out parseValue);

            if (isNumber)
            {
                if (parseValue > 0 && parseValue < 10)
                {
                    return Brushes.RoyalBlue;
                }
                else
                {
                    return Brushes.LimeGreen;
                }
            }
            else if (v == "?")
            {
                return Brushes.Orange;
            }
            else if (v == "Pass" || v == "Coffee")
            {
                return Brushes.DimGray;
            }
            else if(v == "1/2")
            {
                return Brushes.MediumBlue;
            }
            else if(v == "pending")
            {
                return Brushes.Gainsboro;
            }
            else if(v == "ready")
            {
                return Brushes.CornflowerBlue;
            }

            return Brushes.Gold;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
