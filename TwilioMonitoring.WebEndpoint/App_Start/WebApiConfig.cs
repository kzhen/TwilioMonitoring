using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TwilioMonitoring.WebEndpoint
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      //config.Formatters.Remove();
      //var a = config.Formatters.Select(x => x.GetType() == typeof(System.Net.Http.Formatting.JsonMediaTypeFormatter)).FirstOrDefault();
      var jsonMediaTypeFormatters = GlobalConfiguration.Configuration.Formatters
    .Where(x => x.SupportedMediaTypes
    .Any(y => y.MediaType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase)))
    .ToList();

      foreach (var formatter in jsonMediaTypeFormatters)
      {
        GlobalConfiguration.Configuration.Formatters.Remove(formatter);
      }
      //config.Formatters.Remove(a);

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
    }
  }
}
