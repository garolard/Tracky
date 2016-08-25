using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Tracky.ViewModels;
using TraktApiSharp.Objects.Get.Shows;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tracky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            this.InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var show = (TraktShow)e.Parameter;
            var ctx = this.DataContext as DetailViewModel;
            await ctx.OnNavigatedTo(show);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
            {
                var service = ConnectedAnimationService.GetForCurrentView();
                service.GetAnimation("SelectedShow").TryStart(this.Poster);
            }
        }

        //private async void Background_OnImageOpened(object sender, RoutedEventArgs e)
        //{
        //    await Background.Blur(duration: 500, value: 5, delay: 200).StartAsync();
        //}
    }
}
