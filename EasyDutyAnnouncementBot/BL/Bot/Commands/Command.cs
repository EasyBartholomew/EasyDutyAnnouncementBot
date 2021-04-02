using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public abstract class Command
    {
        public virtual string Name => this.GetType().Name.Replace(nameof(Command), string.Empty);

        public virtual bool RequireInit { get; } = true;

        public virtual CommandStatus LastStatus { get; set; }

        public virtual IList<ChatMemberStatus> WhoCanExecute => _whoCanExecute;

        public abstract Task Execute(TelegramBotClient client, Message message);

        public virtual async Task<bool> CanExecute(TelegramBotClient client, Message message)
        {
            var sender = await client.GetChatMemberAsync(message.Chat.Id, message.From.Id);

            if (this.WhoCanExecute.Contains(sender.Status) && !message.From.IsBot)
                return true;

            return false;
        }

        public virtual bool RecognizeSelf(string textMessage)
        {
            return Regex.IsMatch(textMessage,
                $"^\\/{this.Name}(@{AppSettings.BotName})?\\s*$",
                RegexOptions.IgnoreCase);
        }

        protected Command(params ChatMemberStatus[] whoCanExecute)
        {
            _whoCanExecute = new List<ChatMemberStatus>(whoCanExecute);
        }

        protected Command() : this(ChatMemberStatus.Creator, ChatMemberStatus.Administrator)
        { }

        private readonly List<ChatMemberStatus> _whoCanExecute;
    }
}
