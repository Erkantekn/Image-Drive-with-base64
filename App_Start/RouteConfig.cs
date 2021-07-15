using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinalDrive_v2._0
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.RouteExistingFiles = true;
            routes.MapRoute(
                name: "staticFileRoute",
                url: "images/users/{kAdi}/{file}",
                defaults: new { controller = "Home", action = "Login" }
            );




            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
