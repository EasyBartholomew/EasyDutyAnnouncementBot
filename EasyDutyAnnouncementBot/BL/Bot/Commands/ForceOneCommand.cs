using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class ForceOneCommand : StudentTextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            if (message.ReplyToMessage != null)
            {
                await base.TakeData(client, message.ReplyToMessage);

                _userId = message.ReplyToMessage.From.Id;
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Для выполнения данной команды необходимо ответить на сообщение!");
                LastStatus = CommandStatus.Failure;
                return;
            }

            if (LastStatus != CommandStatus.AwaitExecuting)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Пришли студента следующим сообщением)");
                LastStatus = CommandStatus.AwaitNextMessage;
            }
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"{Student} был проассоциирован с {(uint)_userId}");

            Student.UserId = _userId;
            _userId = 0;
            LastStatus = CommandStatus.Success;
        }

        public ForceOneCommand() : base(false)
        { }

        private int _userId;
    }
}