using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            var service = ConnectedAnimationService.GetForCurrentView();

            service.PrepareToAnimate("SelectedShow", posterImage);

            Frame.Navigate(typeof(DetailPage), show);
        }

        private async void SearchBox_OnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            var query = args.QueryText;
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
