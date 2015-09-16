using Newtonsoft.Json;
using RssFeedCmd.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Net;
using System.Xml;
using System.IO;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;


namespace RssFeedCmd
{
    public class RssFeedProcessor
    {
        // TODO : replace by your documentDB endpoint
        private static string EndpointUrl = "<<YOUR DOCDB ENDPOINT>>";
        private static string AuthorizationKey = "<<YOUR KEY>>";

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
                FeedData feed = await GetFeedAsync(blog);
                this.Feeds.Add(feed);
            }

            
            //var json = JsonConvert.SerializeObject(this.Feeds);

        }

        private async Task<FeedData> GetFeedAsync(string feedUriString)
        {
            
          
            try
            {
                Console.WriteLine("FEED " + feedUriString);
                Uri feedUri = new Uri(feedUriString);
                WebClient wcFeed = new WebClient();
                Console.WriteLine("    downloading ...");
                wcFeed.Encoding = Encoding.UTF8;
                var feedStr = wcFeed.DownloadString(feedUri);
                XmlReader xmlReader = XmlReader.Create(new StringReader(feedStr));

                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                
                FeedData feedData = new FeedData();

                feedData.Title = feed.Title.Text;
                feedData.Description = feed.Description.Text;
                
                feedData.PubDate = DateTime.Now;
                
                Console.WriteLine("   parsing ...");
                foreach (SyndicationItem item in feed.Items)
                {
                    Console.Write(".");
                    FeedItem feedItem = new FeedItem();
                    feedItem.Title = item.Title.Text;
                    feedItem.TitleWords = item.Title.Text.ToLower().Split(new char[] {' ',',','.'});

                    ///feedItem.PubDate = item.PublishDate.DateTime;
                    feedItem.Day = item.PublishDate.Day;
                    feedItem.Month = item.PublishDate.Month;
                    feedItem.Year = item.PublishDate.Year;



                    feedItem.Author = (item.Authors.Count>0)?item.Authors[0].Name.ToString():"";
                    // Handle the differences between RSS and Atom feeds.
                    //if (feed.SourceFormat == SyndicationFormat.Atom10)
                    //{
                    //    feedItem.Content = item.Content.Text;
                    //    feedItem.Link = new Uri(item.Id);
                    //}
                    //else if (feed.SourceFormat == SyndicationFormat.Rss20)
                    //{
                    feedItem.Content = item.Summary == null ? "" : item.Summary.Text;
                        feedItem.Link = item.Links[0].Uri;
                    //}
                    feedData.Items.Add(feedItem);
                }
                Console.WriteLine("   Done.");
                return feedData;
            }
            catch (Exception)
            {
                Console.WriteLine("   ERROR");
                return null;
            }
        }

        const string databaseId = "msfteeblogs";
        const string collectionIdPost = "blogPosts";
        const string collectionIdBlogs = "blogs";

        public async void UploadFeedsToDocumentDB()
        {
          
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);  
            
            Database database = client.CreateDatabaseQuery()
                .Where(db => db.Id == databaseId)
                .AsEnumerable()
                .FirstOrDefault(); 
            if (database==null) 
            { 
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId }); 
                Console.WriteLine("Created Database: id - {0} and selfLink - {1}", database.Id, database.SelfLink); 
            }


            DocumentCollection collectionBlog = client.CreateDocumentCollectionQuery(database.CollectionsLink)
                .Where(doccoll => doccoll.Id == collectionIdBlogs)
                .AsEnumerable()
                .FirstOrDefault();
            if (collectionBlog == null)
            {
                collectionBlog = await client.CreateDocumentCollectionAsync(database.SelfLink,
                    new DocumentCollection { Id = collectionIdBlogs });
                Console.WriteLine("Created Collection {0}.", collectionBlog);
            }

            DocumentCollection collectionPosts = client.CreateDocumentCollectionQuery(database.CollectionsLink)
                .Where(doccoll => doccoll.Id == collectionIdPost)
                .AsEnumerable()
                .FirstOrDefault();
            if (collectionPosts == null)
            {
                collectionPosts = await client.CreateDocumentCollectionAsync(database.SelfLink,
                    new DocumentCollection { Id = collectionIdPost });
                Console.WriteLine("Created Collection {0}.", collectionPosts);
            }

            foreach(var f in this.Feeds)
            {
                Console.WriteLine(f.Title);

                foreach (var post in f.Items)
                {
                    Console.Write(".");
                    try
                    {
                        await client.CreateDocumentAsync(collectionPosts.SelfLink, post);
                    }
                    catch(Exception ex)
                    { Console.WriteLine("X"); }
                }

                f.Items.Clear();
                await client.CreateDocumentAsync(collectionBlog.SelfLink, f);
                Console.WriteLine();
            }
        }


        public async void TestQueryDocument()
        {

            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Database database = client.CreateDatabaseQuery()
                .Where(db => db.Id == databaseId)
                .AsEnumerable()
                .FirstOrDefault();
          
            DocumentCollection collectionBlog = client.CreateDocumentCollectionQuery(database.CollectionsLink)
                .Where(doccoll => doccoll.Id == collectionIdBlogs)
                .AsEnumerable()
                .FirstOrDefault();


            DocumentCollection collectionPosts = client.CreateDocumentCollectionQuery(database.CollectionsLink)
                .Where(doccoll => doccoll.Id == collectionIdPost)
                .AsEnumerable()
                .FirstOrDefault();

            /******************************************************/
            var blogs = from b in client.CreateDocumentQuery<FeedData>(collectionBlog.SelfLink)
                           select b;

            Console.WriteLine("BLOGS : ");
            foreach(var b in blogs)
            {
                Console.WriteLine("* " + b.Title);
            }

            Console.WriteLine();


            var nbPosts = (
                from b in client.CreateDocumentQuery<FeedItem>(collectionPosts.SelfLink)
                where b.Year == 2015 && b.Month == 5 
                select new { b.TitleWords, b.Title }).ToList();
            Console.WriteLine(" {0} billets dans la base.", nbPosts.Count);


            var g = client.CreateDocumentQuery(collectionPosts.SelfLink, " SELECT * FROM FeedItem");

        }
    }
}
