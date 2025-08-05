using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using GrapeCity.ActiveReports.Web;

namespace vms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            this.UseReporting(settings =>
            {
                settings.UseFileStore(new DirectoryInfo("Specify the path to the directory with reports"));
                settings.UseCompression = true;
            });
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}