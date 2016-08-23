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
        public MainPage()
        {
            this.InitializeComponent();

            Shows = new ObservableCollection<TraktShow>();
        }
        
        public ObservableCollection<TraktShow> Shows { get; set; }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var client = new TraktClient("2a2f9d290074c6384c5836364cc670827c082cab515d721c7b87938324c2eb70");
            var searchResults = await client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, "The Get Down");

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            foreach (var show in fullShows)
            {
                Shows.Add(show);
            }

            base.OnNavigatedTo(e);
        }

        private void GridElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var element = sender as Grid;
            var posterImage = element.FindDescendant<ImageEx>();
            var show = element.DataContext as TraktShow;

            var service = ConnectedAnimationService.GetForCurrentView();

            service.PrepareToAnimate("SelectedShow", posterImage);

            Frame.Navigate(typeof(DetailPage), show);
        }
    }
}
