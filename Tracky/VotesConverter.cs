using System;
using Windows.UI.Xaml.Data;

namespace Tracky
{
    public class VotesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var votes = (int) value;

            if (votes < 1000) return ((int) value).ToString("###") + " votes";
            else return (((int) value)/1000).ToString("##") + "k votes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}