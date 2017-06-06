using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace EyeScopeAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = "welcome", id = UrlParameter.Optional }
            );
        }
    }
}
