using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using devTd15amsService.DataObjects;
using devTd15amsService.Models;

namespace devTd15amsService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new devTd15amsInitializer());
        }
    }

    public class devTd15amsInitializer : ClearDatabaseSchemaIfModelChanges<devTd15amsContext>
    {
        protected override void Seed(devTd15amsContext context)
        {
            List<BlogSubscription> todoItems = new List<BlogSubscription>
            {
                //new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                //new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (BlogSubscription todoItem in todoItems)
            {
                context.Set<BlogSubscription>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

