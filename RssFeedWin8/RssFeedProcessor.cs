using Newtonsoft.Json;
using RssFeedWin8.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace RssFeedWin8
{
    public class RssFeedProcessor
    {
        private List<string> _blogsUrl = new List<string>();
        private ObservableCollection<FeedData> _Feeds = new ObservableCollection<FeedData>();
        public ObservableCollection<FeedData> Feeds
        {
            get
            {
                return this._Feeds;
            }
        }

        public RssFeedProcessor()
        {

        }

        private void AddBlogsToList()
        {
            _blogsUrl.Add("http://blogs.msdn.com/b/devosaure/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/davrous/atom.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/visualstudio/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/visualstudioalm/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/crm/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/windowsazurestorage/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/eternalcoding/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/mvpawardprogram/rss.aspx");
            _blogsUrl.Add("http://channel9.msdn.com/Feeds/RSS");
            _blogsUrl.Add("http://blogs.msdn.com/b/azuremobile/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/ie/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/dohollan/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/directx/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/emargraff/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/pierlag/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/jdupuy/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/mim/rss.aspx");
            _blogsUrl.Add("http://blogs.msdn.com/b/stephe/rss.aspx");
        }

        public async void GetFeedsAsync()
        {
            AddBlogsToList();

            foreach (var blog in _blogsUrl)
            {
                Task<FeedData> feed = GetFeedAsync(blog);
                this.Feeds.Add(await feed);
            }

            
            var json = JsonConvert.SerializeObject(this.Feeds);

        }

        private async Task<FeedData> GetFeedAsync(string feedUriString)
        {
            // using Windows.Web.Syndication;
            SyndicationClient client = new SyndicationClient();
            Uri feedUri = new Uri(feedUriString);

            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);

                // This code is executed after RetrieveFeedAsync returns the SyndicationFeed.
                // Process it and copy the data we want into our FeedData and FeedItem classes.
                FeedData feedData = new FeedData();

                feedData.Title = feed.Title.Text;
                if (feed.Subtitle != null && feed.Subtitle.Text != null)
                {
                    feedData.Description = feed.Subtitle.Text;
                }
                // Use the date of the latest post as the last updated date.
                feedData.PubDate = feed.Items[0].PublishedDate.DateTime;

                foreach (SyndicationItem item in feed.Items)
                {
                    FeedItem feedItem = new FeedItem();
                    feedItem.Title = item.Title.Text;
                    feedItem.PubDate = item.PublishedDate.DateTime;
                    feedItem.Author = item.Authors[0].Name.ToString();
                    // Handle the differences between RSS and Atom feeds.
                    if (feed.SourceFormat == SyndicationFormat.Atom10)
                    {
                        feedItem.Content = item.Content.Text;
                        feedItem.Link = new Uri(item.Id);
                    }
                    else if (feed.SourceFormat == SyndicationFormat.Rss20)
                    {
                        feedItem.Content = item.Summary.Text;
                        feedItem.Link = item.Links[0].Uri;
                    }
                    feedData.Items.Add(feedItem);
                }
                return feedData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
