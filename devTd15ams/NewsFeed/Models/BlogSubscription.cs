using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFeed.Models
{
    public class BlogSubscription
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "blogname")]
        public string BlogName { get; set; }
    }
}
