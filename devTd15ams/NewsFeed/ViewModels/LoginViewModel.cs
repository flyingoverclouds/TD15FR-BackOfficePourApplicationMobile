using MCS.MVVM;
using Microsoft.WindowsAzure.MobileServices;
using NewsFeed.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NewsFeed.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            LoginFbCommand = new RelayCommand(() => Authentication("fb"));
            LoginMsCommand = new RelayCommand(() => Authentication("ms"));
        }

        public RelayCommand LoginFbCommand { get; private set; }
        public RelayCommand LoginMsCommand { get; private set; }

        private MobileServiceUser currentUser;

        private async void Authentication(string mode)
        {
            if (mode == "fb")
            {
                // Facebook
                await AuthenticateAsync(MobileServiceAuthenticationProvider.Facebook);

                if (currentUser != null)
                {
                    await new MessageDialog("Bienvenue \r\n" + currentUser.UserId, "Connecté FCB").ShowAsync();
                }
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(HomePage), null);
            }
            else if (mode == "ms")
            {
                // Microsoft
                await AuthenticateAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

                //if (currentUser != null)
                //{
                //    await new MessageDialog("Bienvenue \r\n" + currentUser.UserId, "Connecté MS").ShowAsync();
                //}
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(HomePage), null);
            }
        }

        private async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            string message = null;
            try
            {
                currentUser = await App.MobileService
                    .LoginAsync(provider);

            }
            catch (InvalidOperationException ioe)
            {
                currentUser = null;
                message = "ERROR : " + ioe.Message;
            }
            if (!string.IsNullOrEmpty(message))
                await new MessageDialog(message, "ERREUR").ShowAsync();
        }
    }
}
