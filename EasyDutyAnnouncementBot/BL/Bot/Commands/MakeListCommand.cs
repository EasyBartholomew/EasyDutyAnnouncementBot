using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class MakeListCommand : TextListArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли список в формате \"Фамилия Имя\" тех, кого нужно добавить)");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            try
            {
                foreach (var user in DataList)
                {
                    var splitData = user.Trim().Split(' ');

                    if (splitData.Length != 2)
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                        "Пришли список в формате \"Фамилия Имя\" тех, кого нужно добавить)");
                        LastStatus = CommandStatus.AwaitNextMessage;
                        return;
                    }

                    var student = platoon.CreateStudent(splitData[0], splitData[1]);
                }
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    ex.Message);

                LastStatus = CommandStatus.Failure;
                return;
            }


            await client.SendTextMessageAsync(message.Chat.Id,
                "Список студентов обновлён, теперь он выглядит так:");
            await DutyBot.Commands.Single(
                c => c.GetType() == typeof(ListCommand))
                .Execute(client, message);

            LastStatus = CommandStatus.Success;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
               "В качестве списка можно прислать только текстовое сообщения, " +
               "разделяя их при помощи пробела, запятой или новой строки.");

            LastStatus = CommandStatus.AwaitNextMessage;
        }
    }
}
