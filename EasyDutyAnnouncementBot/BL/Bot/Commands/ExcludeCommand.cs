using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class ExcludeCommand : TextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли фамилию того, кого надо исключить из списка)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                    "В качестве фамилии можно отправить только текст)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var queue = platoon.DutyQueue;

            if (Data.Trim().ToLower() == "all")
            {
                platoon.DutyQueue.ExcludeAll();
                LastStatus = CommandStatus.Success;
                return;
            }

            try
            {
                queue.Exclude(Data);
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    ex.Message);

                LastStatus = CommandStatus.Failure;
                return;
            }

            var exStudent = platoon.Students.First(s => s.RecognizeSelf(Data));

            await client.SendTextMessageAsync(message.Chat.Id,
                   $"Студент {exStudent} исключён из списка дежурных.");

            LastStatus = CommandStatus.Success;
        }
    }
}