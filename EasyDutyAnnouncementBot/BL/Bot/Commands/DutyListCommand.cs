using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EasyDutyAnnouncementBot.BL.Models;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class DutyListCommand : Command
    {
        public override async Task Execute(TelegramBotClient client, Message message)
        {
            var sb = new StringBuilder("<b>Список дежурных</b>");

            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            if (platoon.DutyQueue.IsEmpty)
                sb.Append("<b> пуст!</b>");
            else
            {
                sb.Append("<b>:</b>\n\n");

                var pivot = DateTime.UtcNow.AddHours(3).GetNextDate(platoon.TagetDay);

                foreach (var student in platoon.DutyQueue)
                {
                    sb.Append($"<b>{pivot.ToLocalStringFormat()}</b> {student};\n");
                    pivot = pivot.AddDays(7);
                }

                sb.Remove(sb.Length - 2, 2);
                sb.Append(".");
            }

            await client.SendTextMessageAsync(message.Chat.Id, sb.ToString(), ParseMode.Html);

            LastStatus = CommandStatus.Success;
        }

        public DutyListCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
