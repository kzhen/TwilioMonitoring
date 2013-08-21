using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;

namespace TwilioMonitoring.WebEndpoint
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    private IContainer BuildMVCContainer()
    {
      var builder = new ContainerBuilder();


      builder.Register<RabbitMQRepository.IRabbitMQRepository>(m => new RabbitMQRepository.RabbitMQRepository()).SingleInstance();
      
      builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

      return builder.Build();
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      var container = BuildMVCContainer();
      var resolver = new AutofacWebApiDependencyResolver(container);
      GlobalConfiguration.Configuration.DependencyResolver = resolver;

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}