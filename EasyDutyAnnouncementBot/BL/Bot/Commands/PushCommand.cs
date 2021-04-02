using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class PushCommand : TextArgCommand
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

            var splitArgs = Data.Split(new char[] { ',', ';' },
                StringSplitOptions.RemoveEmptyEntries);

            if ((splitArgs.Length < 1) || (splitArgs.Length > 2))
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Передано неверное число аргументов.\n" +
                    "Допустимый синтаксис: \"Фамилия\", количество раз; \"Фамилия\"");
                LastStatus = CommandStatus.AwaitNextMessage;
                return;
            }

            var surname = splitArgs.First();
            var success = UInt32.TryParse(splitArgs.Last(), out uint count);

            try
            {
                if ((splitArgs.Length == 2) && success && (count != 0))
                    queue.PushCurrentDuty(surname, count);
                else
                    queue.PushCurrentDuty(surname);
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
