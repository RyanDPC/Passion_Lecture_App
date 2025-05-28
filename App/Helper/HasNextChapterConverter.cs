using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using VersOne.Epub;
using VersOne.Epub.Schema;   // pour EpubLocalTextContentFile

namespace App.Helper
{
    public class HasNextChapterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // On s'attend à recevoir une ObservableCollection<EpubLocalTextContentFile> 
            // et un index en parameter
            if (value is ObservableCollection<EpubLocalTextContentFile> chapters
                && parameter is string indexString
                && int.TryParse(indexString, out int index))
            {
                return chapters.Count > 0 && index < chapters.Count - 1;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
