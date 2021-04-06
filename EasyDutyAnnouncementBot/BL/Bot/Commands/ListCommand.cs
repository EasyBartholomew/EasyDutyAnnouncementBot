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

            if (platoon.Count() == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Список студентов пуст!");
            }
            else
            {
                var list = platoon.Select(s => s.ToString()).ToArray();
                var sb = new StringBuilder("<b>Личный состав:</b>\n\n");

                for (var i = 0; i < list.Length; i++)
                {
                    sb.Append(i + 1)
                    .Append(". ")
                    .Append(list[i])
                    .Append(";\n");
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append('.');

                await client.SendTextMessageAsync(message.Chat.Id, sb.ToString(), ParseMode.Html);
            }

            LastStatus = CommandStatus.Success;
        }

        public ListCommand() : base()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
