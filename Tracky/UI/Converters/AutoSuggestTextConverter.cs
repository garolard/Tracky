using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Tracky.UI.Converters
{
    public class AutoSuggestTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (AutoSuggestBoxTextChangedEventArgs) value;
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}