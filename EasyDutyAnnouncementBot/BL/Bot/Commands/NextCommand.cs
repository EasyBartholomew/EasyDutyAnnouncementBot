using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class NextCommand : Command
    {
        public override async Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var queue = platoon.DutyQueue;

            try
            {
                queue.PopCurrentDuty();

                await client.SendTextMessageAsync(message.Chat.Id,
                    $"Хорошо теперь дежурит {queue.GetCurrentDuty()}.");

            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
                LastStatus = CommandStatus.Failure;
            }

            LastStatus = CommandStatus.Success;
        }
    }
}
