using System;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EasyDutyAnnouncementBot.BL;
using EasyDutyAnnouncementBot.BL.Bot;
using EasyDutyAnnouncementBot.Controllers;

namespace EasyDutyAnnouncementBot
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            HostingEnvironment.RegisterObject(new HostingEnvironmentRegisteredObject());

            if (!SerializeHelper.LoadConfiguration())
            {
                HomeController.Message = "Configuration not found!";
                return;
            }

            DutyBot.Init();
            SerializeHelper.DeserializeAll();
        }
    }
}
