using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class AddListCommand : TextListArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли список фамилий тех, кого нужно добавить в список дежурных)");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var sb = new StringBuilder("Список дежурных обновлён, теперь он выглядит так:");

            if ((DataList.Length == 1) && DataList[0].Trim() == "all")
            {
                var surnames = platoon.Students.Select(s => s.Surname);

                sb.Insert(0, "Все студенты были добалены в список дежурных!\n");

                platoon.DutyQueue.AddList(surnames);
            }
            else
            {
                try
                {
                    platoon.DutyQueue.AddList(DataList.Select(s => s.Trim()));
                }
                catch (Exception ex)
                {
                    sb.Insert(0, $"{ex.Message}\n");
                }
            }

            LastStatus = CommandStatus.Success;

            await client.SendTextMessageAsync(message.Chat.Id, sb.ToString());

            await DutyBot.Commands
                .Single(c => c.GetType() == typeof(DutyListCommand))
                .Execute(client, message);
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "В качестве списка можно прислать только текстовое сообщение, " +
                "разделяя студентов при помощи пробела, запятой или новой строки.");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public AddListCommand()
        { }
    }
}
