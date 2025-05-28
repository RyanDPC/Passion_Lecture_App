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

        // Convertir une ImageSource en Byte[]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ImageSource imageSource)
            {
                // Si l'ImageSource est un StreamImageSource, nous pouvons essayer d'accéder au flux sous-jacent
                if (imageSource is StreamImageSource streamImageSource)
                {
                    try
                    {
                        using (var stream = streamImageSource.Stream(CancellationToken.None).Result) 
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                
                                stream.CopyTo(memoryStream);

                                
                                return memoryStream.ToArray();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la conversion de l'image en byte[] : {ex.Message}");
                        return null;
                    }
                }
            }

            return null;
        }

    }
}
