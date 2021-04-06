using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    internal class ChangeCommand : AddCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                  "Пришли обновлённый список дежурных)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var platoon = DutyBot.GetGroupByChatId(message.Chat.Id).Platoon;

            platoon.DutyQueue.ExcludeAll();

            return base.ExecuteWith(client, message);
        }
    }
}