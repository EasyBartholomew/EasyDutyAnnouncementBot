using System;

namespace EasyDutyAnnouncementBot.BL.Models
{
    public class NoInstanceException : Exception
    {
        public NoInstanceException() : base()
        { }

        public NoInstanceException(string message) : base(message)
        { }
    }
}
