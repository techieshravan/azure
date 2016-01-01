using MyFixIt.App_Start;
using MyFixIt.Logging;
using MyFixIt.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyFixIt
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            DependenciesConfig.RegisterDependencies();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            PhotoService photoService = new PhotoService(new Logger());
            photoService.CreateAndConfigureAsync();

            DbConfiguration.SetConfiguration(new MyFixIt.Persistence.EFConfiguration());
        }
    }
}
