using System;
using System.Globalization;
using System.Windows.Data;

namespace WordTeacher.Converters
{
    public class ValueToCenterPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return Binding.DoNothing;

            var positionX = (double)values[0];
            var width = (double)values[1];

            return positionX - width/2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
