using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class PushCommand : StudentTextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await DutyBot.Commands.Single(c => c.GetType() == typeof(CurrentCommand))
                .Execute(client, message);
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли фамилию того, кого нужно назначить)");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                    "В качестве студента можно отправить только текст)");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var queue = platoon.DutyQueue;

            try
            {
                queue.PushCurrent(Student.ToString());
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
                LastStatus = CommandStatus.Failure;
                return;
            }

            var current = queue.GetCurrentDuty();

            await client.SendTextMessageAsync(message.Chat.Id,
                $"Теперь дежурит {current}.");

            LastStatus = CommandStatus.Success;
        }
    }
}
