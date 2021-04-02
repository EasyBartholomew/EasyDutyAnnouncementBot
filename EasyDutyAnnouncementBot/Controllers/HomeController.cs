using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace EasyDutyAnnouncementBot.Controllers
{
    public class HomeController : Controller
    {
        public static string Message { get; set; } = null;

        public string Index()
        {
            if (Message != null)
                return Message;

            var info = new FileInfo(Assembly.GetExecutingAssembly().Location).CreationTime;

            return $"EasyDutyAnnouncementBot-dev | " +
                $"Current build {info.ToShortDateString()} | {info.ToLongTimeString()}";
        }
    }
}