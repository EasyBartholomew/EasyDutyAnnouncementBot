using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class HelloCommand : Command
    {
        public override bool RequireInit { get; } = false;

        public override async Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                $"Привет, {message.From.FirstName}!", replyToMessageId: message.MessageId);
            LastStatus = CommandStatus.Success;
        }

        public HelloCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
