﻿using System.Web;
using System.Web.Mvc;

namespace Azure.Diagnostics.Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}