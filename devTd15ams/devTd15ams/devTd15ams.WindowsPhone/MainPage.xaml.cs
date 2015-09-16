using devTd15ams.DataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace devTd15ams
{
    public sealed partial class MainPage : Page
    {
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //await RefreshTodoItems();
        }

        private MobileServiceCollection<BlogSubscription, BlogSubscription> items;
        private IMobileServiceTable<BlogSubscription> todoTable = App.MobileService.GetTable<BlogSubscription>();


        private MobileServiceUser currentUser;
        private async void butLoginFCB_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (currentUser!=null)
            {
                await new MessageDialog("Vous êtes DEJA connecté en tant que \r\n" + currentUser.UserId, "DEJA Connecté").ShowAsync();
            }

            await AuthenticateAsync(MobileServiceAuthenticationProvider.Facebook);

            if (currentUser!=null)
                await new MessageDialog("Bienvenue \r\n"+currentUser.UserId, "Connecté FCB").ShowAsync();

        }

      
        private async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
           
            string message=null;
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

        private async void butLoginMS_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
             if (currentUser!=null)
            {
                await new MessageDialog("Vous êtes DEJA connecté en tant que \r\n" + currentUser.UserId, "DEJA Connecté").ShowAsync();
            }

            await AuthenticateAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

            if (currentUser!=null)
                await new MessageDialog("Bienvenue \r\n"+currentUser.UserId, "Connecté AAD").ShowAsync();
        }

      
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task InsertTodoItem(BlogSubscription todoItem)
        {
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);
        }

        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                items = await todoTable
                    //.Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                if (e.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return;
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async Task UpdateCheckedTodoItem(BlogSubscription item)
        {
                      
            //await todoTable.UpdateAsync(item); 
            await todoTable.DeleteAsync(item);
            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;

            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new BlogSubscription { BlogName = "BLOG " + DateTime.Now.ToString() };
            await InsertTodoItem(todoItem);
        }

        private async void testApi_Click(object sender, RoutedEventArgs e)
        {
            string err=null;
            // GET /api/search/?criterias=TOTO
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("criterias", DateTime.Now.ToString());
                var result = await App.MobileService.InvokeApiAsync<List<FeedItem>>
                    ("search", System.Net.Http.HttpMethod.Get, param);

                string res = "Résultat : \r\n";
                foreach (var r in result)
                    res += r.ToString() + "\r\n";

                await new MessageDialog(res, "Votre recherche").ShowAsync();
            }
            catch(Exception ex)
            {
                err = ex.Message;
                
            }
            if (err!=null)
            {
                await new MessageDialog(err, "EXCEPTION").ShowAsync();
            }
        }


        private async void butRegisterForNotif_Click(object sender, RoutedEventArgs e)
        {
            await InitNotificationsAsync();
        }

      

        private async Task InitNotificationsAsync()
        {
            //try
            //{
            //    var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            //    await App.MobileService.GetPush().RegisterAsync(channel.Uri);
            //}
            //catch(Exception ex)
            //{
            //    string s = ex.ToString();
            //}
            
            // Request a push notification channel.
            var channel = await PushNotificationChannelManager
                .CreatePushNotificationChannelForApplicationAsync();
           
            // Register for notifications using the new channel
            System.Exception exception = null;
            try
            {
                var push = App.MobileService.GetPush();

                await push.UnregisterAsync();
              
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
                var push = App.MobileService.GetPush();
                await push.RegisterAsync(channel.Uri);
               
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
