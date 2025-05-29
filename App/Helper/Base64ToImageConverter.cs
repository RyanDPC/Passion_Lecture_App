using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace App.Helper
{
    public class Base64ToImageConverter : IValueConverter
    {
        // Convertit une chaîne Base64 en ImageSource
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrEmpty(base64String))
            {
                try
                {
                    // Si ta chaîne contient un préfixe "data:image/xxx;base64,", il faut le retirer :
                    var commaIndex = base64String.IndexOf(',');
                    if (commaIndex >= 0)
                        base64String = base64String.Substring(commaIndex + 1);

                    byte[] bytes = System.Convert.FromBase64String(base64String);
                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                catch (Exception)
                {
                    // En cas d'erreur dans la conversion, retourne une image par défaut
                    return ImageSource.FromFile("a0.png");
                }
            }
            // Valeur nulle ou vide => image par défaut
            return ImageSource.FromFile("a0.png");
        }

        // Pas utilisé dans ce cas (binding à sens unique)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
