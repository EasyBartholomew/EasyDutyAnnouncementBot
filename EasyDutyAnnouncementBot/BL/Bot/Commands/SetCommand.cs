using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class SetCommand : TextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Пришли фамилию того, кого нужно перенести))");

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

            try
            {
                queue.SetCurrentDuty(Data);
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                   ex.Message);

                LastStatus = CommandStatus.Failure;
                return;
            }

            await client.SendTextMessageAsync(message.Chat.Id,
                   $"Студент {queue.GetCurrentDuty()} утсановлен в качестве текущего дежурного.");

            LastStatus = CommandStatus.Success;
        }
    }
}