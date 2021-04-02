using System;


namespace EasyDutyAnnouncementBot.BL.Bot
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException() : base()
        { }

        public InvalidIdException(string message) : base(message)
        { }
    }
}