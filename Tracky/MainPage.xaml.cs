﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Tracky.ViewModels;
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
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                var ctx = DataContext as MainViewModel;
                await ctx.ClearStateAsync();
            }
        }

        private void GridElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var element = sender as Grid;
            var show = element.DataContext as TraktShow;

            Frame.Navigate(typeof(DetailPage), show);
        }
    }
}
