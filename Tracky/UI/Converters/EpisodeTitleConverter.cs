using System;
using Windows.UI.Xaml.Data;
using TraktApiSharp.Objects.Get.Shows.Episodes;

namespace Tracky.UI.Converters
{
    public class EpisodeTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var episode = (TraktEpisode) value;
            if (episode == null) return string.Empty;
            return $"{episode.SeasonNumber}x{episode.Number} {episode.Title}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}