using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WordTeacher.Converters
{
    public class HiddenTopConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return Binding.DoNothing;

            var isHidden = (bool)values[0];
            if (isHidden)
                return 0.0;

            return -(double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
