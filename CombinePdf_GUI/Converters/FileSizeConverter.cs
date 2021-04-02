using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CombinePdf_GUI.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        private const int MegabyteInBytes = 1048576;
        private const int GigbyteInBytes = 1073741824;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long tmpFileSize)
            {
                double fileSize = tmpFileSize;
                double size = 0;
                if (fileSize >= GigbyteInBytes)
                {
                    size = fileSize / 1024 / 1024 / 1024;
                    return $"{size} GB";
                }
                else if (fileSize >= MegabyteInBytes)
                {
                    size = fileSize / 1024 / 1024;
                    return $"{size} MB"; 
                }
                else
                {
                    size = Math.Round((double)(fileSize / 1024), MidpointRounding.AwayFromZero);
                    return $"{size} KB";
                }
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fileSizeStr)
            {
                string[] tokens = fileSizeStr.Split(' ');
                double.TryParse(tokens[0], out double fileSize);
                return fileSize * 1000;
            }
            else
            {
                return 0;
            }
        }
    }
}
