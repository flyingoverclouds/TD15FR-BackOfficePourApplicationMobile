using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace devTd15ams
{
    public class BlogSubscription
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "blogname")]
        public string BlogName { get; set; }

        //[JsonProperty(PropertyName = "text")]
        //public string Text { get; set; }

        //[JsonProperty(PropertyName = "complete")]
        //public bool Complete { get; set; }
    }
}
