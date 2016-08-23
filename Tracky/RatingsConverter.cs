using System;
using Windows.UI.Xaml.Data;

namespace Tracky
{
    public class RatingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var rating = (float) value;
            rating = rating*10;
            return Math.Truncate(rating).ToString("###") + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}