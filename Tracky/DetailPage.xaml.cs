using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using TraktApiSharp;
using TraktApiSharp.Objects.Basic;
using TraktApiSharp.Objects.Get.Shows;
using TraktApiSharp.Objects.Get.Shows.Episodes;
using TraktApiSharp.Requests.Params;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tracky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        private readonly TraktClient _client;

        public DetailPage()
        {
            this.InitializeComponent();
            _client = new TraktClient(Constants.TraktId);
            Window.Current.Activated += CurrentOnActivated;
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CustomizeTitleBar();

            var show = (TraktShow)e.Parameter;
            DataContext = show;

            AnimatePoster();

            await LoadActorsAsync();
            await LoadRecentEpisodesAsync();
            await Background
                .Blur(value: 5, duration: 1000, delay: 400)
                .StartAsync();
        }

        private void CustomizeTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBarOnLayoutMetricsChanged;
            TitleBar.Height = coreTitleBar.Height;
            Window.Current.SetTitleBar(MainTitleBar);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
                }
            }
        }

        private void AnimatePoster()
        {
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Poster");
            if (animation != null)
            {
                Poster.Opacity = 0;
                Poster.ImageOpened += (sender_, e_) =>
                {
                    Poster.Opacity = 1;
                    animation.TryStart(Poster);
                };
            }
        }

        private async Task LoadActorsAsync()
        {
            var show = DataContext as TraktShow;
            var showPeople = await _client.Shows.GetShowPeopleAsync(show.Ids.Trakt.ToString(), new TraktExtendedOption { Full = true, Images = true });
            ActorsGrid.ItemsSource = showPeople.Cast;
        }

        private async Task LoadRecentEpisodesAsync()
        {
            var show = DataContext as TraktShow;
            var showSeasons = await _client.Seasons.GetAllSeasonsAsync(show.Ids.Slug);
            if (!showSeasons.Any()) return;

            var lastSeason =
                await
                    _client.Seasons.GetSeasonAsync(show.Ids.Slug, showSeasons.Last().Number.Value,
                        new TraktExtendedOption() { Episodes = true, Images = true });
            var recentEpisodes = lastSeason.Reverse().Take(3).ToList();
            EpisodesList.ItemsSource = recentEpisodes;
        }

        private void CurrentOnActivated(object sender, WindowActivatedEventArgs windowActivatedEventArgs)
        {
            if (windowActivatedEventArgs.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                MainTitleBar.Opacity = 1;
            }
            else
            {
                MainTitleBar.Opacity = 0.5;
            }
        }

        private void CoreTitleBarOnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleBar.Height = sender.Height;
            RightMask.Width = sender.SystemOverlayRightInset;
        }
    }
}
