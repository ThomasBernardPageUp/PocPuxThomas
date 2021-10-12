using System;
using System.Globalization;
using Xamarin.Forms;

namespace PocPuxThomas.Converters
{
    public class SexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.Default;
            switch (value)
            {
                case "Male":
                    color = Color.MediumAquamarine;
                    break;
                case "Female":
                    color = Color.MediumOrchid;
                    break;
                case "unknown":
                    color = Color.AliceBlue;
                    break;
                case "Genderless":
                    color = Color.NavajoWhite;
                    break;
                default:
                    color = Color.Red;
                    break;
            }
            return color.ToHex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
