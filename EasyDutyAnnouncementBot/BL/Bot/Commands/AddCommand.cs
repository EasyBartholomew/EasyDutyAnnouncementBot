using EasyDutyAnnouncementBot.BL.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class AddCommand : TextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Хорошо, пришли его фамилию)");

            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var queue = platoon.DutyQueue;
            Student student;

            try
            {
                student = queue.AddDuty(Data);
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
                return;
            }

            await client.SendTextMessageAsync(message.Chat.Id, $"Студент " +
                $"{student} успешно добавлен!");
        }

        public AddCommand() : base()
        { }
    }
}
