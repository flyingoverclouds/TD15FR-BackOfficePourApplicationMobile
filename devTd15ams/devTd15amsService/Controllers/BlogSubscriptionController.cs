using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using devTd15amsService.DataObjects;
using devTd15amsService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace devTd15amsService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class BlogSubscriptionController : TableController<BlogSubscription>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            devTd15amsContext context = new devTd15amsContext();
            DomainManager = new EntityDomainManager<BlogSubscription>(context, Request, Services);
        }

        // GET tables/BlogSubscription
        public IQueryable<BlogSubscription> GetAllTodoItems()
        {
            var currentUser = User as ServiceUser;

            return Query().Where(i => i.UserId == currentUser.Id);
        }

        // GET tables/BlogSubscription/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BlogSubscription> GetTodoItem(string id)
        {

            return Lookup(id);
        }

        // PATCH tables/BlogSubscription/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BlogSubscription> PatchTodoItem(string id, Delta<BlogSubscription> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/BlogSubscription
        public async Task<IHttpActionResult> PostTodoItem(BlogSubscription item)
        {
            var currentUser = User as ServiceUser;
            item.UserId = currentUser.Id;

            BlogSubscription current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/BlogSubscription/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTodoItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}