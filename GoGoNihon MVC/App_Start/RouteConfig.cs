using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GoGoNihon_MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            

            routes.MapMvcAttributeRoutes();


            routes.MapRoute(
                name: "careers",
                url: "careers-work-with-us/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "careers", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "careers2",
                url: "{languageCode}/careers-work-with-us/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "careers", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "terms-and-conditions",
                url: "terms-and-conditions/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "terms", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "terms-and-conditions2",
                url: "{languageCode}/terms-and-conditions/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "terms", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "japanese-language-schools-learn",
                url: "{languageCode}/japanese-language-schools-learn/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "listLanguageSchools", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "language schools listing",
                url: "japanese-language-schools-learn/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "listLanguageSchools", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "accomadation-with-language",
                url: "{languageCode}/japan-apartments-rent-guesthouse-homestay/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "accomodation", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "accomadation",
                url: "japan-apartments-rent-guesthouse-homestay/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "accomodation", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Account",
                url: "account/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "school",
                url: "{languageCode}/school/{location}/{url}",
                defaults: new { languageCode = "en", controller = "School", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "school2",
                url: "school/{location}/{url}",
                defaults: new { languageCode = "en", controller = "School", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "schools",
                url: "{languageCode}/schools/",
                defaults: new { languageCode = "en", controller = "Home", action = "schools", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "schools2",
                url: "schools/",
                defaults: new { languageCode = "en", controller = "Home", action = "schools", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{languageCode}/{controller}/{action}/{id}",
                defaults: new { languageCode = "en", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
