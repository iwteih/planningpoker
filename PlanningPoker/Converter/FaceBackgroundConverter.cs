using PlanningPoker.Entity;
using System;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlanningPoker.Converter
{
    public class FaceBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            String v = value.ToString().ToLower();
            int parseValue;
            bool isNumber = int.TryParse(v, out parseValue);

            if (isNumber)
            {
                if (parseValue >= 0 && parseValue < 10)
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
            else if (v == "1/2")
            {
                return Brushes.RoyalBlue;
            }
            else if (IsInCardStatus(v))
            {
                return buildImageBrush(v);
            }

            return Brushes.Gold;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsInCardStatus(string v)
        {
            foreach (string e in Enum.GetNames(typeof(CardStatus)))
            {
                if (e.ToLower() == v)
                {
                    return true;
                }
            }

            return false;
        }

        private ImageBrush buildImageBrush(string status)
        {
            ImageBrush imageBrush = new ImageBrush();
            string fullName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            string assemblyName = fullName.Substring(0, fullName.IndexOf(","));
            string imagePath = string.Format("pack://application:,,,/{0};component/Properties/../Resources/{1}.png", assemblyName, status);
            imageBrush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            imageBrush.Stretch = Stretch.Uniform;
            return imageBrush;
        }
    }
}
