using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using App.Models;
namespace App.Helper
{
    public class HasNextChapterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Chapter> chapters && parameter is int index)
                return chapters != null && index < chapters.Count - 1;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
