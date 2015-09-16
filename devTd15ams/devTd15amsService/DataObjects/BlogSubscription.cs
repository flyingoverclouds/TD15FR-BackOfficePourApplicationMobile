using Microsoft.WindowsAzure.Mobile.Service;

namespace devTd15amsService.DataObjects
{
    public class BlogSubscription : EntityData
    {
        public string UserId { get; set; }

        public string BlogName { get; set; }

        //public string Text { get; set; }

        //public bool Complete { get; set; }
    }
}