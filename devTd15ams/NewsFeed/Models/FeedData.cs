using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFeed.Model
{
    public class FeedData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }

        private List<FeedItem> _Items = new List<FeedItem>();
        public List<FeedItem> Items
        {
            set
            {
                this._Items = value;
            }
            get
            {
                return this._Items;
            }
        }

        public FeedData(List<FeedItem> items)
        {
            Items = items;
        }
    }
}
