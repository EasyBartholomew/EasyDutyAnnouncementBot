using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class WhoCommand : Command
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;

            if (message.ReplyToMessage != null)
            {
                var userId = message.ReplyToMessage.From.Id;

                if (userId == (await client.GetMeAsync()).Id)
                {
                    await client.SendTextMessageAsync(chatId, "Я дежурный бот)");

                    LastStatus = CommandStatus.Success;
                    return;
                }

                if (message.ReplyToMessage.From.IsBot)
                {
                    await client.SendTextMessageAsync(chatId, "Это один из ботов, который нам помогает)");

                    LastStatus = CommandStatus.Success;
                    return;
                }

                var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;
                var student = platoon.FirstOrDefault(s => s.UserId == userId);

                if (student == null)
                {
                    await client.SendTextMessageAsync(chatId, "Указанный пользователь не был проассоциирован((");

                    LastStatus = CommandStatus.Failure;
                    return;
                }

                await client.SendTextMessageAsync(chatId, $"\"{message.ReplyToMessage.From.FirstName}\" это {student}");

                LastStatus = CommandStatus.Success;
            }
            else
            {
                await client.SendTextMessageAsync(chatId,
                    "Для выполнения данной команды необходимо ответить на сообщение студента, которого хочешь узнать!");
                LastStatus = CommandStatus.Failure;
                return;
            }
        }

        public WhoCommand()
        {
            WhoCanExecute.Add(Telegram.Bot.Types.Enums.ChatMemberStatus.Member);
        }

    }
}