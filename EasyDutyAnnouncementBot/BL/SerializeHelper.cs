using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using EasyDutyAnnouncementBot.BL.Bot;
using EasyDutyAnnouncementBot.BL.Models;


namespace EasyDutyAnnouncementBot.BL
{
    public static class SerializeHelper
    {
        public static void SerializeGroup(Group group)
        {
            try
            {
                var chatId = group.ChatId;
                var directoryPath = $"{LOCAL}\\{chatId}";

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var writerSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    WriteEndDocumentOnClose = true
                };

                using (var xmlWriter = XmlWriter.Create($"{directoryPath}\\{GROUP_FILE_NAME}", writerSettings))
                {
                    dataContractSerializer.WriteObject(xmlWriter, group);
                }
            }
            catch (Exception)
            {
                //Write logic later
                throw;
            }
        }

        public static bool BeenSerialized(ulong chatId)
        {
            return File.Exists($"{LOCAL}\\{chatId}\\{GROUP_FILE_NAME}");
        }

        public static Group DeserializeGroup(ulong chatId)
        {
            var directoryPath = $"{LOCAL}\\{chatId}";
            var groupPath = $"{directoryPath}\\{GROUP_FILE_NAME}";

            if (!Directory.Exists(directoryPath))
                throw new SerializationException("Эта группа не была сериализована!");

            if (!File.Exists(groupPath))
                throw new SerializationException("Сериализация была произведена некорректно!");

            try
            {
                Group group;

                using (var xmlReader = XmlReader.Create(groupPath))
                {
                    group = dataContractSerializer.ReadObject(xmlReader) as Group;
                }
               
                return group;
            }
            catch (Exception)
            {
                //Write logic later
                throw;
            }
        }

        public static void SerializeAll()
        {
            var groups = DutyBot.Groups;

            foreach (var group in groups)
            {
                SerializeGroup(group);
            }
        }

        public static void DeserializeAll()
        {
            if (!Directory.Exists(LOCAL))
                return;

            var ids = new DirectoryInfo(LOCAL)
                .GetDirectories()
                .Select<DirectoryInfo, UInt64>(di =>
                {
                    if (UInt64.TryParse(di.Name, out UInt64 id))
                        return id;
                    return 0;
                }).Where(v => v != 0);

            foreach (var id in ids)
            {
                if (!DutyBot.IsKnownChatId((long)id))
                {
                    DutyBot.groups.Add(DeserializeGroup(id));
                }
            }
        }

        public static bool LoadConfiguration()
        {
            var document = new XmlDocument();
            var path = $"{ROOT}\\{CONFIGURATION_FILE_NAME}";

            if (!File.Exists(path))
                return false;

            document.Load(path);

            var token = document.GetElementsByTagName("token")[0].InnerText;
            var name = document.GetElementsByTagName("name")[0].InnerText;

            AppSettings.BotName = name;
            AppSettings.Token = token;

            return true;
        }

        static SerializeHelper()
        {
            dataContractSerializer = new DataContractSerializer(typeof(Group),
                new Type[]
                {
                    typeof(Student),
                    typeof(Platoon),
                    typeof(CustomQueue)
                });
        }


        private const string ROOT = @"C:\Apps\EasyDutyAnnouncementBot\localdata";
        private const string LOCAL = @"C:\Apps\EasyDutyAnnouncementBot\localdata\groups";
        private const string CONFIGURATION_FILE_NAME = "Configuration.xml";
        private const string GROUP_FILE_NAME = "Group.xml";
        private static readonly DataContractSerializer dataContractSerializer;
    }
}