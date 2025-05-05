using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Helper
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        // Convertit un byte[] en ImageSource
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            // Image par défaut si pas de données
            return ImageSource.FromFile("a0.png");
        }

        // Pas d’opération inverse nécessaire
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
