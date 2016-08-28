using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
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
        private TraktShow _selectedShow;

        public MainPage()
        {
            this.InitializeComponent();
            _client = new TraktClient(Constants.TraktId);
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CustomizeTitleBar();
        }

        private static void CustomizeTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = null;
                }
            }
        }

        private void AdaptiveGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var container = ShowsGrid
                .FindDescendant<GridView>()
                .ContainerFromItem(e.ClickedItem) as GridViewItem;
            if (container != null)
            {
                var root = (FrameworkElement) container.ContentTemplateRoot;
                var image = (UIElement) root.FindDescendant<Image>();
                if (image != null)
                {
                    ShowsGrid.FindDescendant<GridView>().UpdateLayout();
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Poster", image);
                }
            }

            _selectedShow = e.ClickedItem as TraktShow;
            Frame.Navigate(typeof(DetailPage), e.ClickedItem);
        }

        private async void SearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;

            var query = sender.Text;
            if (string.IsNullOrEmpty(query))
            {
                SearchBox.ItemsSource = new ObservableCollection<TraktShow>()
                {
                    new TraktShow { Title = "No result" }
                };
                return;
            }

            var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            if (fullShows.Any())
            {
                SearchBox.ItemsSource = fullShows;
            }
            else
            {
                SearchBox.ItemsSource = new ObservableCollection<TraktShow>()
                {
                    new TraktShow { Title = "No result" }
                };
            }
        }

        private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ShowsGrid.ItemsSource = null;

            var query = string.Empty;

            query = args.ChosenSuggestion == null ? args.QueryText : ((TraktShow)args.ChosenSuggestion).Title;

            var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            ShowsGrid.ItemsSource = fullShows;
        }

        
    }
}
