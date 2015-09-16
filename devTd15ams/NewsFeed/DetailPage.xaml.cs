using NewsFeed.Common;
using NewsFeed.Model;
using NewsFeed.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NewsFeed
{
    public sealed partial class DetailPage : Page
    {
        private NavigationHelper navigationHelper;
        private DetailViewModel _vm;

        public DetailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                _vm = new DetailViewModel(e.Parameter);
                this.DataContext = _vm; 

                FeedItem item = e.Parameter as FeedItem;
                this.WvContent.NavigateToString(item.Content);
            }
        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.FavoritesCommand.Execute(null);
        }

    }
}
