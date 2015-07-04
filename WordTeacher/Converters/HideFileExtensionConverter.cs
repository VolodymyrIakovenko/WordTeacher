using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace WordTeacher.Converters
{
    public class HideFileExtensionConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var fileNames = value as IEnumerable<string>;
            if (fileNames != null)
                return fileNames.Select(Path.GetFileNameWithoutExtension);

            return Path.GetFileNameWithoutExtension(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var fileNames = value as IEnumerable<string>;
            if (fileNames != null) 
                return fileNames.Select(x => x + ".xml");

            return value + ".xml";
        }
    }
}
