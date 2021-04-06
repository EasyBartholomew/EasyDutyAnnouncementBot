using System;
using System.Linq;
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
            var commands = Properties.Resources.Сommands;
            var userStatus = (await client.GetChatMemberAsync(message.Chat.Id, message.From.Id)).Status;

            var avaibleCommands = DutyBot.Commands
                .Where(c => c.WhoCanExecute.Contains(userStatus))
                .Select(c => c.Name.ToLower());

            var avaibleList = string.Join("\n", commands
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => avaibleCommands
                .Contains(s.Split(new char[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]))
                .Select(s => s.Insert(0, "/")));

            await client.SendTextMessageAsync(message.Chat.Id,
                $"<b>Доступные команды:</b>\n\n{avaibleList}", ParseMode.Html);

            LastStatus = CommandStatus.Success;
        }

        public HelpCommand()
        {
            WhoCanExecute.Add(ChatMemberStatus.Member);
        }
    }
}