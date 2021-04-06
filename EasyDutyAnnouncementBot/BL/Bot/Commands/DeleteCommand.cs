using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using EasyDutyAnnouncementBot.BL.Models;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class DeleteCommand : StudentTextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли студента, кого нужно исключить из списка студентов)\n" +
                "(данный человек будет также исключён из списка дежурных)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            if (Data.Trim().ToLower() == "all")
            {
                platoon.RemoveAll();

                await client.SendTextMessageAsync(chatId, "Все студенты были удалены из списка!");

                LastStatus = CommandStatus.Success;
                return;
            }

            try
            {
                platoon.RemoveStudent(Student.ToString());
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                  ex.Message);

                LastStatus = CommandStatus.Success;
                return;
            }

            await client.SendTextMessageAsync(message.Chat.Id,
                $"Студент {Student} был успешно исключен.");

            LastStatus = CommandStatus.Success;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                    "В качестве студента можно отправить только текст)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }
    }
}