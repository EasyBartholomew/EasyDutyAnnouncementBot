using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class ListCommand : Command
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            if (platoon.Students.Count == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Список студентов пуст.");
            }
            else
            {
                var sb = new StringBuilder();
                var list = platoon.Students.Select(s => s.ToString());

                foreach (var student in list)
                {
                    sb.Append(student);
                    sb.Append(";\n");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append('.');

                await client.SendTextMessageAsync(message.Chat.Id, sb.ToString());
            }

            LastStatus = CommandStatus.Success;
        }

        public ListCommand() : base()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
