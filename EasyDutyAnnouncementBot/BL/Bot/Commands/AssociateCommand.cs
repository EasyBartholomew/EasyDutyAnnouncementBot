using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class AssociateCommand : StudentTextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            _userId = message.From.Id;

            var platoon = DutyBot.GetGroupByChatId(message.Chat.Id).Platoon;

            if (platoon.Count(s => s.UserId == _userId) != 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    $"Студент {platoon.First(s => s.UserId == _userId)} уже был проассоцирован с данным аккаунтом!\n" +
                    "Если это ошибка, то обратись к командиру)", replyToMessageId: message.MessageId); ;

                LastStatus = CommandStatus.Failure;
                return;
            }

            if (message.ReplyToMessage != null)
            {
                await base.TakeData(client, message.ReplyToMessage);
            }

            if (LastStatus != CommandStatus.AwaitExecuting)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Пришли студента следующим сообщением)",
                    replyToMessageId: message.MessageId);
                LastStatus = CommandStatus.AwaitNextMessage;
            }
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            if (Student.UserId != 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Данный студент уже был проассоцирован с другим id!\n" +
                    "Обратись к командиру)", replyToMessageId: message.MessageId);
                LastStatus = CommandStatus.Failure;

                return;
            }

            Student.UserId = _userId;
            await client.SendTextMessageAsync(message.Chat.Id, $"{Student} был проассоциирован с id{(uint)_userId}",
                replyToMessageId: message.MessageId);
            _userId = 0;

            LastStatus = CommandStatus.Success;
        }

        public AssociateCommand() : base(false)
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }

        private int _userId;
    }
}