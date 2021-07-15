using System;
using System.Web;
using System.Web.Mvc;

namespace FinalDrive_v2._0
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
       

    }
}
