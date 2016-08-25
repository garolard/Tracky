using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using TraktApiSharp.Objects.Get.Shows;

namespace Tracky.UI.Converters
{
    public class AutoSuggestChosenSuggestionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (AutoSuggestBoxSuggestionChosenEventArgs) value;
            return args.SelectedItem as TraktShow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}