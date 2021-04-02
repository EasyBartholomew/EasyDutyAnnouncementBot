using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class StartCommand : Command
    {
        public override bool RequireInit { get; } = false;

        public async override Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;

            if (!DutyBot.IsKnownChatId(chatId))
            {
                DutyBot.CreateGroupFor(chatId);

                await client.SendTextMessageAsync(chatId,
                    "Начнём) Для получения списка команд используйте /help");
            }
        }
    }
}