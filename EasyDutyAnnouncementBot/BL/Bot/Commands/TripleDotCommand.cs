using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class TripleDotCommand : Command
    {
        public override bool RecognizeSelf(string textMessage)
        {
            return textMessage == "...";
        }

        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Дааа, у меня тоже нет слов...");
        }

        public TripleDotCommand() : base()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}
