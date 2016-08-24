using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using TraktApiSharp;
using TraktApiSharp.Enums;
using TraktApiSharp.Objects.Get.Shows;
using TraktApiSharp.Requests.Params;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tracky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly TraktClient _client;

        public MainPage()
        {
            this.InitializeComponent();
            _client = new TraktClient(Constants.TraktId);
            Shows = new ObservableCollection<TraktShow>();
        }
        
        public ObservableCollection<TraktShow> Shows { get; set; }

        private void GridElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var element = sender as Grid;
            var posterImage = element.FindDescendant<ImageEx>();
            var show = element.DataContext as TraktShow;

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
            {
                var service = ConnectedAnimationService.GetForCurrentView();
                service.PrepareToAnimate("SelectedShow", posterImage);
            }

            Frame.Navigate(typeof(DetailPage), show);
        }

        private async void SearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var query = sender.Text;
                if (string.IsNullOrEmpty(query))
                {
                    SearchBox.ItemsSource = new TraktShow[] { new TraktShow() { Title = "No results" } };
                    return;
                }

                var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

                var tasks = searchResults
                    .Select(result => result.Show.Ids.Trakt.ToString())
                    .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                    .ToList();

                var fullShows = await Task.WhenAll(tasks);
                if (fullShows.Any())
                    SearchBox.ItemsSource = fullShows;
                else
                    SearchBox.ItemsSource = new TraktShow[] {new TraktShow() {Title = "No results"}};
            }
        }

        private void SearchBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selectedShow = (TraktShow)args.SelectedItem;
            sender.Text = selectedShow.Title;
        }

        private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var query = string.Empty;

            if (args.ChosenSuggestion != null)
            {
                var selectedShow = (TraktShow) args.ChosenSuggestion;
                query = selectedShow.Title;
            }
            else
            {
                query = sender.Text;
            }

            var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            Shows.Clear();
            foreach (var show in fullShows)
            {
                Shows.Add(show);
            }
        }
    }
}
