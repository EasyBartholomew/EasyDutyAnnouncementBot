using EasyDutyAnnouncementBot.BL;
using System.Web.Hosting;

namespace EasyDutyAnnouncementBot
{
    public class HostingEnvironmentRegisteredObject : IRegisteredObject
    {
        public void Stop(bool immediate)
        {
            SerializeHelper.SerializeAll();
        }
    }
}