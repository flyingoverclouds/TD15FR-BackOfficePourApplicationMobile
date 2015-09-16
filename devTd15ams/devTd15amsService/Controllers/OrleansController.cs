using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace devTd15amsService.Controllers
{
     [AuthorizeLevel(AuthorizationLevel.User)] 
    public class OrleansController : ApiController
    {
        public ApiServices Services { get; set; }


        // GET /api/orleans/?msg=TOTO
        public async Task<string> Get([FromUri]string msg = "toto")
        {
                        await SendNotification(msg);

            return "OK";
        }

        private async Task SendNotification(string message)
        {
           

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

            GooglePushMessage googlePushMessage = new GooglePushMessage();
            googlePushMessage.CollapseKey = "Blog post";
            googlePushMessage.Data.Add("message", message);
            googlePushMessage.DelayWhileIdle = true;
            googlePushMessage.TimeToLiveInSeconds = TimeSpan.FromHours(1).Seconds;

            try
            {

                var resultWindows = await Services.Push.SendAsync(windowsPushMessage);
                Services.Log.Info(resultWindows.State.ToString());

                var resultGoogle = await Services.Push.SendAsync(googlePushMessage);
                Services.Log.Info(resultGoogle.State.ToString());
            }
            catch (System.Exception ex)
            {
                Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
            }
            
        }


         

      }
}
