using Microsoft.WindowsAzure.MobileServices;
using MCS.MVVM;
using NewsFeed.Common;
using NewsFeed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace NewsFeed.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private FeedData _blogs;
        public FeedData Blogs 
        { 
            get
            {
                return _blogs;
            }
            set
            {
                _blogs = value;
                NotifyPropertyChanged("Blogs");
            }
        }

        public HomeViewModel()
        {
            ParseJsonUrl();
            NotifsCommand = new RelayCommand(() => InitNotificationsAsync2());
        }

        public RelayCommand NotifsCommand { get; private set; }

        private async void ParseJsonUrl()
        {
            //using (HttpClient client = new HttpClient())
            //{
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("criterias", DateTime.Now.ToString());

                var result = await App.MobileService.InvokeApiAsync<List<FeedItem>>
                    ("search", System.Net.Http.HttpMethod.Get, param);

                Blogs = new FeedData(result);


         
        }

        private async Task InitNotificationsAsync()
        {

            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            System.Exception exception = null;

            try
            {
                //await App.MobileService.GetPush().UnregisterAsync();
                await App.MobileService.GetPush().RegisterAsync(channel.Uri);
                //await App.MobileService.GetPush().RegisterNativeAsync(channel.Uri, new string[] { "project orleans","azure mobile services" });
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                var dialog = new MessageDialog(exception.Message, "Registering Channel URI");
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            else
            {
                await new MessageDialog("SUCCESS", "Registering Channel URI").ShowAsync();
            }
        }

        private async Task InitNotificationsAsync2()
        {
            // Request a push notification channel.
            var channel = await PushNotificationChannelManager
                .CreatePushNotificationChannelForApplicationAsync();

            // Register for notifications using the new channel
            System.Exception exception = null;
            try
            {
                await App.MobileService.GetPush().UnregisterAsync();

            }
            catch (System.Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                var dialog = new MessageDialog(exception.Message, "UNREGISTERING Channel URI");
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            try
            {
                exception = null;
                await App.MobileService.GetPush().RegisterAsync(channel.Uri);

                await new MessageDialog("SUCCESS", "Registering Channel URI").ShowAsync();
            }
            catch (System.Exception ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                var dialog = new MessageDialog(exception.Message, "Registering Channel URI");
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }    

    }
}
