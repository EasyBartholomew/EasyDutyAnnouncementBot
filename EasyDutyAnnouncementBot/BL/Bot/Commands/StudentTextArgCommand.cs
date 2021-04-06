using EasyDutyAnnouncementBot.BL.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EasyDutyAnnouncementBot.BL.Bot.Commands
{
    public abstract class StudentTextArgCommand : TextArgCommand
    {
        public Student Student { get; protected set; }

        public bool IgnoreAllArgument { get; protected set; }

        public async override Task OnTextData(TelegramBotClient client, Message message)
        {
            var chatId = message.Chat.Id;
            var platoon = DutyBot.GetGroupByChatId(chatId)?.Platoon;

            if (platoon == null)
                throw new InvalidIdException("Взаимодействие с данной группой не было инициировано.");

            if (_keyboardShown)
            {
                var markupMessage = await client.SendTextMessageAsync(chatId, $"Выбран {message.Text}",
                    replyMarkup: new ReplyKeyboardRemove());
                await client.DeleteMessageAsync(chatId, markupMessage.MessageId);

                _keyboardShown = false;
            }

            if (Data.Trim().ToLower() == "all")
            {
                if (IgnoreAllArgument)
                {
                    Student = null;
                    LastStatus = CommandStatus.AwaitExecuting;
                    return;
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "Данная команда не поддерживает \"all\"!\n" +
                        "Введите имя или фамилию студента)");

                    LastStatus = CommandStatus.AwaitNextMessage;
                    return;
                }
            }

            Student[] students = null;

            if (_students == null)
            {
                students = _students = platoon.GetStudents(Data);
            }
            else
            {
                students = _students.Where(s => s.RecognizeSelf(Data)).ToArray();
            }

            if (students.Length == 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"{Data} отсутсвует в списке студентов!");
                LastStatus = CommandStatus.AwaitNextMessage;
                _students = null;
                return;
            }

            if (students.Length != 1)
            {
                var studentsMessage = new StringBuilder("По введённым данным невозможно однозначно определить студента!\n" +
                    "Кого именно имели в виду?\n");

                foreach (var student in students)
                {
                    studentsMessage.Append($"{student},\n");
                }

                studentsMessage
                    .Remove(studentsMessage.Length - 2, 2)
                    .Append('.');

                bool equalsName = true;

                if (students[0].Surname == students[1].Surname)
                    equalsName = false;

                var keyboard = new ReplyKeyboardMarkup(students
                    .Select(s => new KeyboardButton(equalsName ? s.Surname : s.Name)), true, true)
                { Selective = true };

                await client.SendTextMessageAsync(message.Chat.Id,
                    $"{studentsMessage}", replyMarkup: keyboard, replyToMessageId: message.MessageId);

                _keyboardShown = true;

                LastStatus = CommandStatus.AwaitNextMessage;
                return;
            }

            _students = null;
            Student = students.Single();
            LastStatus = CommandStatus.AwaitExecuting;
        }

        public override void OnCancel()
        {
            _students = null;
            Student = null;
        }

        protected StudentTextArgCommand(bool ignoreAllArgument)
        {
            this.IgnoreAllArgument = ignoreAllArgument;
        }

        protected StudentTextArgCommand() : this(true)
        { }

        private Student[] _students;
        private bool _keyboardShown;
    }
}