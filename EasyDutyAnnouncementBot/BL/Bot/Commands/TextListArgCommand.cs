using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public abstract class TextListArgCommand : TextArgCommand
    {
        public string[] DataList { get; private set; }

        public char[] Separators
        {
            get => _separators;

            set
            {
                _separators = value ?? throw new ArgumentNullException(nameof(value));

                if (value.Length == 0)
                    throw new ArgumentException("Separators list can not be empty!");
            }
        }

        public override Task OnTextData(TelegramBotClient client, Message message)
        {
            var list = this.Data.Split(this.Separators, StringSplitOptions.RemoveEmptyEntries);

            if (list.Length >= 1)
            {
                DataList = list;
                return this.OnTextDataList(client, message);
            }

            return base.OnTextDataError(client, message);
        }

        public virtual Task OnTextDataList(TelegramBotClient client, Message message)
        {
            LastStatus = CommandStatus.AwaitExecuting;

            return Task.CompletedTask;
        }

        protected TextListArgCommand(params char[] separators)
        {
            Separators = separators;
        }

        protected TextListArgCommand() : this(TextListArgCommand.DefaultSeparators)
        { }

        protected TextListArgCommand(params ChatMemberStatus[] whoCanExecute) : base(whoCanExecute)
        { }

        private char[] _separators;

        public static char[] DefaultSeparators { get; } = new char[] { '\n', '\t', ',', ';', '.' };
    }
}