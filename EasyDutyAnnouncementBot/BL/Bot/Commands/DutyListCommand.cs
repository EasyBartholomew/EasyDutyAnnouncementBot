using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class DutyListCommand : Command
    {
        public override async Task Execute(TelegramBotClient client, Message message)
        {
            var sb = new StringBuilder("Список дежурных");

            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            if (platoon.DutyQueue.IsEmpty)
                sb.Append(" пуст!");
            else
            {
                sb.Append(":\n\n");

                foreach (var student in platoon.DutyQueue)
                {
                    sb.Append($"{student};\n");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append(".");
            }

            await client.SendTextMessageAsync(message.Chat.Id, sb.ToString());

            LastStatus = CommandStatus.Success;
        }

        public DutyListCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
