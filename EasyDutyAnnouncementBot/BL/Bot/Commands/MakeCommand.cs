using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    class MakeCommand : TextArgCommand
    {
        public async override Task Execute(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Введите фамилию и имя студента:");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task ExecuteWith(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            var student = platoon.CreateStudent(_surname, _name);

            await client.SendTextMessageAsync(message.Chat.Id, $"Студент {student} был добавлен в список.");

            LastStatus = CommandStatus.Success;
        }

        public async override Task OnTextDataError(TelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                   "В качестве студента можно отправить только текст в формате \"Фамилия Имя\".");
            LastStatus = CommandStatus.AwaitNextMessage;
        }

        public async override Task OnTextData(TelegramBotClient client, Message message)
        {
            var surnameName = Data
                .Trim()
                .Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (surnameName.Length != 2)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                   "В качестве студента можно отправить только текст в формате \"Фамилия Имя\".");
                return;
            }

            _surname = surnameName[0];
            _name = surnameName[1];

            LastStatus = CommandStatus.AwaitExecuting;
        }

        public async override Task TakeData(TelegramBotClient client, Message message)
        {
            if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message?.Text))
            {

                return;
            }

            var surnameName = message.Text
                .Trim()
                .Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (surnameName.Length != 2)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                   "В качестве студента можно отправить только текст в формате \"Фамилия Имя\".");
                return;
            }

            _surname = surnameName[0];
            _name = surnameName[1];


            LastStatus = CommandStatus.AwaitExecuting;
        }

        private string _surname = null;
        private string _name = null;
    }
}
