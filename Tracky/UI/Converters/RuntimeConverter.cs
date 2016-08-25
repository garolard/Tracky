using System;
using Windows.UI.Xaml.Data;

namespace Tracky.UI.Converters
{
    public class RuntimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var runtime = (int?) value;
            return $"{runtime ?? 0} minutes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var runtime = value.ToString();
            return int.Parse(runtime);
        }
    }
}