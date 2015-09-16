using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;
namespace devTd15amsService.ScheduledJobs
{
    // GET /jobs/NotificationAbonnement
    public class NotificationAbonnementJob : ScheduledJob
    {
     
        public override Task ExecuteAsync()
        {
            Services.Log.Info("Sending notification for registration (not implemented)");
            
            // TODO : déclencher la synchro des billets de blogs


            // TODO : ajouter le ciblage des notifications 
            var t = SendNotification("De nouveaux billets de blog sont disponibles.");
            t.Wait();

            return Task.FromResult(true);
        }

        private async Task SendNotification(string message)
        {


           

            try
            {
                #region Windows push notification
                // Create a WNS native toast.
                WindowsPushMessage windowsPushMessage = new WindowsPushMessage
                {
                    XmlPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                                 @"<toast><visual><binding template=""ToastText02"">" +
                                 @"<text id=""1"">Blog post</text>" +
                                 @"<text id=""2"">" + message + @"</text>" +
                                 @"</binding></visual></toast>"
                };

                // Define the XML paylod for a WNS native toast notification 
                // that contains the text of the inserted item.

              
                var resultWindows = await Services.Push.SendAsync(windowsPushMessage);
                //await Services.Push.SendAsync(windowsPushMessage,"azure mobile services");
                Services.Log.Info(resultWindows.State.ToString());
                #endregion


                #region Google Push notification
                GooglePushMessage googlePushMessage = new GooglePushMessage();
                googlePushMessage.CollapseKey = "Blog post";
                googlePushMessage.Data.Add("message", message);
                googlePushMessage.DelayWhileIdle = true;
                googlePushMessage.TimeToLiveInSeconds = TimeSpan.FromHours(1).Seconds;

                var resultGoogle = await Services.Push.SendAsync(googlePushMessage);
                Services.Log.Info(resultGoogle.State.ToString());
                #endregion
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }

        }


    }
}