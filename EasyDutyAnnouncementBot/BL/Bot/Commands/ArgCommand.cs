using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public abstract class ArgCommand : Command
    {
        public Int32 LastUserId { get; protected set; } = -1;

        public bool CheckLastUserId { get; set; } = true;

        public override Task<bool> CanExecute(TelegramBotClient client, Message message)
        {
            LastUserId = message.From.Id;
            return base.CanExecute(client, message);
        }

        public async Task SendData(TelegramBotClient client, Message message)
        {
            if (this.CheckLastUserId)
            {
                if (this.LastUserId == message.From.Id)
                    await TakeData(client, message);
            }
            else
            {
                await TakeData(client, message);
            }
        }

        public abstract Task TakeData(TelegramBotClient client, Message message);

        public abstract Task ExecuteWith(TelegramBotClient client, Message message);

        protected ArgCommand(params ChatMemberStatus[] whoCanExecute) : base(whoCanExecute)
        { }

        protected ArgCommand() : base()
        { }
    }
}
