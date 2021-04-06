using System;


namespace EasyDutyAnnouncementBot.BL.Models
{
    public class CollisionException : Exception
    {
        public CollisionException() : base()
        { }

        public CollisionException(string message) : base(message)
        { }
    }
}