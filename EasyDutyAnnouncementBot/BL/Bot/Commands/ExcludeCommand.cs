using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class ExcludeCommand : StudentTextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли студента, которого надо исключить из списка)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                    "В студента можно отправить только текст)");

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

                await client.SendTextMessageAsync(message.Chat.Id,
                   "Все студенты были исключены из списка дежурных)\nТеперь список дежурных пуст!");

                LastStatus = CommandStatus.Success;
                return;
            }

            try
            {
                queue.Exclude(Student.ToString());
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    ex.Message);

                LastStatus = CommandStatus.Failure;
                return;
            }

            var exStudent = platoon.GetStudent(Data);

            await client.SendTextMessageAsync(message.Chat.Id,
                   $"Студент {exStudent} исключён из списка дежурных.");

            LastStatus = CommandStatus.Success;
        }
    }
}