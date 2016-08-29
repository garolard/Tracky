using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
            Window.Current.Activated += CurrentOnActivated;
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CustomizeTitleBar();

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent(
                    "Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
            {
                BackButton.Width = 48;
                BackButton.Height = 36;
                BackButton.Visibility = Visibility.Visible;
                BackButton.Click += (s, args) => Frame.GoBack();
            }

            var show = (TraktShow)e.Parameter;
            var ctx = this.DataContext as DetailViewModel;
            await ctx.OnNavigatedTo(show);
            var blur = Background.Blur(value: 5, duration: 1000, delay: 400);
            if (blur != null)
                await blur.StartAsync();
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
