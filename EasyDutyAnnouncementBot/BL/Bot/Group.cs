using System;
using System.Runtime.Serialization;
using EasyDutyAnnouncementBot.BL.Models;
using EasyDutyAnnouncementBot.BL.Bot.Commands;


namespace EasyDutyAnnouncementBot.BL.Bot
{
    [DataContract]
    public class Group
    {
        [IgnoreDataMember]
        public Platoon Platoon => _platoon;

        [IgnoreDataMember]
        public UInt64 ChatId => _chatId;

        [IgnoreDataMember]
        public Command LastCommand { get; set; }

        public Group(UInt64 chatId, Platoon platoon)
        {
            _chatId = chatId;
            _platoon = platoon;
        }

        [DataMember(IsRequired = true)]
        private readonly UInt64 _chatId;

        [DataMember(IsRequired = true)]
        private readonly Platoon _platoon;
    }
}