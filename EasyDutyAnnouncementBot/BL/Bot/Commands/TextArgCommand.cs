using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public abstract class TextArgCommand : ArgCommand
    {
        public string Data { get; private set; }

        public override Task TakeData(TelegramBotClient client, Message message)
        {
            if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message?.Text))
            {
                return this.OnTextDataError(client, message);
            }

            Data = message.Text;

            return this.OnTextData(client, message);
        }

        public virtual Task OnTextData(TelegramBotClient client, Message message)
        {
            LastStatus = CommandStatus.AwaitExecuting;

            return Task.CompletedTask;
        }

        public virtual Task OnTextDataError(TelegramBotClient client, Message message)
        {
            LastStatus = CommandStatus.Failure;
            return Task.CompletedTask;
        }

        protected TextArgCommand(params ChatMemberStatus[] whoCanExecute) : base(whoCanExecute)
        { }

        protected TextArgCommand() : base()
        { }
    }
}