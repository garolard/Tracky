using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using TraktApiSharp.Objects.Get.Shows;

namespace Tracky.UI.Converters
{
    public class AutoSuggestQueryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = (AutoSuggestBoxQuerySubmittedEventArgs) value;
            return args.ChosenSuggestion as TraktShow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}