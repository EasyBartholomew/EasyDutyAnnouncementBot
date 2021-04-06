using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EasyDutyAnnouncementBot.BL.Models;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class WhenCommand : Command
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var userId = message.From.Id;

            var platoon = DutyBot.GetGroupByChatId(chatId).Platoon;

            var student = platoon.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {

                await client.SendTextMessageAsync(chatId, "Ты не был проассоциирован!\n" +
                    $"Используй /{nameof(AssociateCommand).Replace("Command", "").ToLower()} для выполнения этого)",
                    replyToMessageId: message.MessageId);

                LastStatus = CommandStatus.Failure;
                return;
            }

            var number = platoon.DutyQueue.ToList().IndexOf(student);

            if (number == -1)
            {
                await client.SendTextMessageAsync(chatId, "Тебя нет в очереди на дежурство!\nПоздравляю)",
                    replyToMessageId: message.MessageId);
                LastStatus = CommandStatus.Success;
                return;
            }

            var target = DateTime.UtcNow
                .AddHours(3)
                .GetNextDate(platoon.TagetDay)
                .AddDays(7 * number)
                .ToLocalStringFormat();

            await client.SendTextMessageAsync(chatId, $"Ты дежуришь <b>{target}</b>)\nНе забудь подготовиться!", ParseMode.Html,
                replyToMessageId: message.MessageId);

            LastStatus = CommandStatus.Success;
        }

        public WhenCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}