using System;
using System.Linq;
using System.Collections.Generic;
using Telegram.Bot;
using EasyDutyAnnouncementBot.BL.Models;
using EasyDutyAnnouncementBot.BL.Bot.Commands;


namespace EasyDutyAnnouncementBot.BL.Bot
{
    public static class DutyBot
    {
        public static TelegramBotClient Client { get; private set; }

        public static IReadOnlyList<Command> Commands => commands.AsReadOnly();

        public static IReadOnlyList<Group> Groups => groups.AsReadOnly();

        public static bool IsKnownChatId(long chatId)
        {
            var group = groups.SingleOrDefault(g => g.ChatId == (ulong)chatId);

            return group != null;
        }

        public static Group CreateGroupFor(long chatId)
        {
            if (IsKnownChatId(chatId))
                throw new ArgumentException(); //Add description here

            if (Groups.Count == MAX_GROUPS)
                throw new ArgumentOutOfRangeException("", "К сожалению лимит бота исчерпан((");

            var group = new Group((ulong)chatId, new Platoon());
            groups.Add(group);

            return group;
        }

        public static Group GetGroupByChatId(long chatId)
        {
            var group = groups.FirstOrDefault(g => g.ChatId == (ulong)chatId);

            return group;
        }

        public static void Init()
        {
            groups = new List<Group>();

            Client = new TelegramBotClient(AppSettings.Token);

            commands = new List<Command>()
            {
                new HelloCommand(),
                new AddCommand(),
                new DutyListCommand(),
                new NextCommand(),
                new CurrentCommand(),
                new PushCommand(),
                new AddListCommand(),
                new CancelCommand(),
                new MakeCommand(),
                new ListCommand(),
                new TripleDotCommand(),
                new MakeListCommand(),
                new SetCommand(),
                new ExcludeCommand(),
                new HelpCommand(),
                new DeleteCommand(),
                new StartCommand()
            };

            Client.StartReceiving();
            Client.OnMessage += OnNewMessage;
        }

        private const Byte MAX_GROUPS = 3;

        private static async void OnNewMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if ((e == null) || (e?.Message == null))
                return;

            var message = e.Message;

            try
            {
                if (message.Text != null)
                {
                    var group = GetGroupByChatId(message.Chat.Id);

                    foreach (var command in Commands)
                    {
                        if (command.RecognizeSelf(message.Text))
                        {
                            if ((group == null) && command.RequireInit)
                            {
                                await Client.SendTextMessageAsync(message.Chat.Id,
                                   "Для выполнения этой команды требуется инициализация!");
                                return;
                            }

                            if (!await command.CanExecute(Client, message))
                            {
                                await Client.SendTextMessageAsync(message.Chat.Id,
                                    "Вы не можете выполнять эту комманду");
                                return;
                            }

                            await command.Execute(Client, message);

                            if (group != null)
                                group.LastCommand = command;

                            return;
                        }
                    }

                    if (group?.LastCommand?.LastStatus == CommandStatus.AwaitNextMessage)
                    {
                        var argCommand = group.LastCommand as ArgCommand;

                        argCommand.SendData(Client, message);

                        if (argCommand.LastStatus == CommandStatus.AwaitExecuting)
                            await argCommand.ExecuteWith(Client, message);
                    }
                }
            }
            catch (Exception ex)
            {
                await Client.SendTextMessageAsync(message.Chat.Id, ex.Message);
            }
        }

        private static List<Command> commands;
        internal static List<Group> groups;
    }
}
