using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class CancelCommand : Command
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Выполнение последней команды отменено.");
            LastStatus = CommandStatus.Success;
        }

        public CancelCommand() : base()
        {
            WhoCanExecute.Add(Telegram.Bot.Types.Enums.ChatMemberStatus.Member);
        }

    }
}
