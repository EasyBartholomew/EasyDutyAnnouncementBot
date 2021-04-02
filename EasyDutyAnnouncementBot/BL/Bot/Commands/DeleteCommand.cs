using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EasyDutyAnnouncementBot.BL.Models;
using System.Collections.Generic;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class DeleteCommand : TextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Введи фамилию того, кого нужно исключить из списка студентов)\n" +
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

                LastStatus = CommandStatus.Success;
                return;
            }

            var student = platoon.Students.FirstOrDefault(s => s.RecognizeSelf(Data));

            if (student == null)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    $"Студент \"{Data}\" отсутсвует в списке студентов!");

                LastStatus = CommandStatus.Success;
                return;
            }

            try
            {
                platoon.DutyQueue.Exclude(Data);
                platoon.RemoveStudent(Data);
            }
            catch (Exception)
            { }

            await client.SendTextMessageAsync(message.Chat.Id,
                $"Студент {Data} был успешно исключен.");

            LastStatus = CommandStatus.Success;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                    "В качестве фамилии можно отправить только текст)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }
    }
}