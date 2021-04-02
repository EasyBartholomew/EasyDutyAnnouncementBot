using System;

namespace EasyDutyAnnouncementBot.BL.Models
{
    [Flags]
    public enum StudentRank : byte
    {
        Student = 1,
        PartCommander = 2,
        PlatoonCommander = 4
    }
}
