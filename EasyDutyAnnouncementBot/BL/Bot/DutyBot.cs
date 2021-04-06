using System;
using System.Linq;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
                throw new InvalidOperationException("Данная группа уже была инициализированна!");

            if (Groups.Count == MAX_GROUPS)
                throw new InvalidOperationException("К сожалению лимит бота исчерпан((");

            var group = new Group((ulong)chatId, new Platoon());
            groups.Add(group);

            return group;
        }

        public static Group GetGroupByChatId(long chatId)
        {
            var group = groups.FirstOrDefault(g => g.ChatId == (ulong)chatId);

            return group;
        }

        public static async void Init()
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
                new CancelCommand(),
                new MakeCommand(),
                new ListCommand(),
                new TripleDotCommand(),
                new SetCommand(),
                new ExcludeCommand(),
                new HelpCommand(),
                new DeleteCommand(),
                new StartCommand(),
                new ForceOneCommand(),
                new AssociateCommand(),
                new WhenCommand(),
                new WhoCommand(),
                new ChangeCommand()
            };


            var avaibleCommands = DutyBot.Commands
                .Where(c => c.WhoCanExecute.Contains(ChatMemberStatus.Member))
                .Select(c => c.Name.ToLower());

            var avaibleList = Properties.Resources.Сommands
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => avaibleCommands
                .Contains(s.Split(new char[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]));

            await Client.SetMyCommandsAsync(
                avaibleList.Select(s =>
                {
                    var commandDescription = s.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (s.Trim()[0] == '#')
                        return null;

                    return new BotCommand()
                    {
                        Command = commandDescription[0].Trim(),
                        Description = commandDescription.Length == 2 ? commandDescription[1].Trim() : "нет описания"
                    };
                })
                .Where(bc => bc != null));

            Client.StartReceiving();
            Client.OnMessage += OnNewMessage;
        }

        private const Byte MAX_GROUPS = 3;

        private static async void OnNewMessage(object sender, MessageEventArgs e)
        {
            if ((e == null) || (e?.Message == null))
                return;

            var message = e.Message;

            try
            {
                if (message.Text != null)
                {
                    var group = GetGroupByChatId(message.Chat.Id);
                    var userId = message.From.Id;

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
                            {
                                if (group.LastCommand == null)
                                    group.LastCommand = new Dictionary<int, Command>();

                                if (group.LastCommand.ContainsKey(userId))
                                    group.LastCommand[userId] = command;
                                else
                                    group.LastCommand.Add(userId, command);
                            }

                            if (command.LastStatus == CommandStatus.AwaitExecuting)
                                await (command as ArgCommand).ExecuteWith(Client, message);

                            return;
                        }
                    }

                    if (group.LastCommand[userId] is ArgCommand argCommand)
                    {
                        if (argCommand.LastStatus == CommandStatus.AwaitNextMessage)
                            await argCommand.SendData(Client, message);
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
