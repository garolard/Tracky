using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Tracky.UI.Converters
{
    public class InverseBoolToVisbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool) value)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility) value == Visibility.Visible)
                return false;
            else
                return true;
        }
    }
}