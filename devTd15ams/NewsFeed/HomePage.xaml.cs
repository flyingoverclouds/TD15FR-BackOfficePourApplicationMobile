using NewsFeed.Common;
using NewsFeed.ViewModels;
using System;
using System.Collections.Generic;
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
    public sealed partial class HomePage : Page
    {
        private NavigationHelper navigationHelper;
        private HomeViewModel _vm;

        public HomePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);

            this.Loaded += HomePage_Loaded;
        }

        void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new HomeViewModel();
            this.DataContext = _vm;
        }

        private void lstData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailPage), this.lstData.SelectedItem);
        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.NotifsCommand.Execute(null);
        }
    }
}
