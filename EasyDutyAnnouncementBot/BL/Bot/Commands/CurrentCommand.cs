using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class CurrentCommand : Command
    {
        public override async Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            try
            {
                var current = platoon.DutyQueue.GetCurrentDuty();

                await client.SendTextMessageAsync(message.Chat.Id, $"Сейчас дежурит {current}.");
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
                LastStatus = CommandStatus.Failure;
            }

            LastStatus = CommandStatus.Success;
        }

        public CurrentCommand()
        {
            WhoCanExecute.Add(Telegram.Bot.Types.Enums.ChatMemberStatus.Member);
        }
    }
}
