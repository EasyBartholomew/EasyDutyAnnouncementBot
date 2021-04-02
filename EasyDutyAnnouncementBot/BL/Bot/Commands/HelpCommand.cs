using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public class HelpCommand : Command
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            var commands = Properties.Resources.commands;

            await client.SendTextMessageAsync(message.Chat.Id,
                $"Доступные комманды:\n\n{commands}");

            LastStatus = CommandStatus.Success;
        }

        public HelpCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}