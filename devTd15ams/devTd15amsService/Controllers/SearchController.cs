using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using devTd15amsService.DataObjects;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

using Microsoft.Azure.Documents.Linq;


namespace devTd15amsService.Controllers
{
     [AuthorizeLevel(AuthorizationLevel.User)] 
    public class SearchController : ApiController
    {
        public ApiServices Services { get; set; }

        // TODO : REPLACE DE THE FOLLOWING SETTINGS BY YOUR OWN SETTINGS

        const string databaseId = "techdays2015blogs"; // TODO : REPLACE by your docDb ID
        const string collectionIdPost = "blogPosts"; // TODO : REPLACE by your docdb posts collection name
        const string collectionIdBlogs = "blogs"; // TODO : REPLACE by your your docdb blog collection name
        const string EndpointUrl = "https://testtd15.documents.azure.com:443/"; // TODO : REPLACE by your docDB endpoint
        
        const string AuthorizationKey = "<<REPLACE BY YOUR KEY>>";
        
        // GET /api/search/?criterias=TOTO
        public List<FeedItem> Get([FromUri]string criterias = "")
        {
            // si criterias = LAST10 -> renvoyé les dix derniere articles
            
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            Database database = client.CreateDatabaseQuery()
                .Where(db => db.Id == databaseId)
                .AsEnumerable()
                .FirstOrDefault();


            DocumentCollection collectionPosts = client.CreateDocumentCollectionQuery(database.CollectionsLink)
                .Where(doccoll => doccoll.Id == collectionIdPost)
                .AsEnumerable()
                .FirstOrDefault();

          

            var posts  = (
                from b in client.CreateDocumentQuery<FeedItem>(collectionPosts.SelfLink)
                where b.Year == 2015 && b.Month >= 1 && b.Day >= 8
                select b).ToList();

            
            return posts;


        }

         
    }
}
