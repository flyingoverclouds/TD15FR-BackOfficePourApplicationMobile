using System.Web.Http;
using System.Web.Routing;

namespace devTd15amsService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}