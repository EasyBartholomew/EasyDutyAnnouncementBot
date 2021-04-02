using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace EasyDutyAnnouncementBot.BL.Models
{
    [DataContract(IsReference = true)]
    public class Student : IComparable<Student>, IEquatable<Student>
    {
        [DataMember]
        public string Surname
        {
            get => _surname;

            set => _surname = CheckAndFormatPrivateData(value, "Фамилия");
        }

        [DataMember]
        public string Name
        {
            get => _name;
            set => _name = CheckAndFormatPrivateData(value, "Имя");
        }

        [DataMember]
        public StudentRank Rank { get; set; }

        public bool RecognizeSelf(string privateData)
        {
            var splitData = privateData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitData.Length == 1)
                return this.Surname == FormatPrivateData(privateData);

            if (splitData.Length == 2)
            {
                splitData = splitData.Select(d => FormatPrivateData(d)).ToArray();

                if ((splitData[0] == this.Surname) && (splitData[1] == this.Name))
                    return true;
                if ((splitData[0] == this.Name) && (splitData[1] == this.Surname))
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{Surname} {Name}";
        }

        public int CompareTo(Student other)
        {
            return this.ToString().CompareTo(other?.ToString());
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Student);
        }

        public bool Equals(Student other)
        {
            if (other == null)
                return false;

            return this.Surname.Equals(other.Surname)
                && this.Name.Equals(other.Name)
                && this.Rank.Equals(other.Rank);
        }

        internal Student(string surname, string name)
        {
            Surname = surname;
            Name = name;
            Rank = StudentRank.Student;
        }

        [IgnoreDataMember]
        private string _surname;

        [IgnoreDataMember]
        private string _name;

        public static char[] ContainsDeniedChars(string privateData)
        {
            var wrongChars = new List<char>();

            foreach (var @char in privateData)
            {
                if (!char.IsLetter(@char))
                    wrongChars.Add(@char);
            }

            if (wrongChars.Count != 0)
                return wrongChars.ToArray();

            return null;
        }

        public static string FormatPrivateData(string unformated)
        {
            var charsToTrim = new char[]
            {
                ' ', '\n', '\t', '\v',
                ',', ';', '.', ':', '?', '!',
                '/', '\\', '\"', '\'', '<', '>'
            };

            //Make string lowercase and delete all whitespaces
            var sb = new StringBuilder(unformated.Trim(charsToTrim).ToLower());

            //Make first letter to be upper
            sb[0] = Char.ToUpper(sb[0]);

            return sb.ToString();
        }

        private static string CheckAndFormatPrivateData(string value, string property)
        {
            if (value == null)
                throw new ArgumentNullException(property,
                    $"{property} не может быть null!");

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{property} не может быть пустой или состоящей из пробелов строкой!");

            var formatted = FormatPrivateData(value);

            var denied = ContainsDeniedChars(formatted);

            if (denied != null)
            {
                var sb = new StringBuilder($"{property} не может содержать следующие символы: ");

                foreach (var @char in denied)
                    sb.Append(@char)
                      .Append(", ");

                sb.Remove(sb.Length - 2, 2)
                  .Append('!');

                throw new ArgumentException(sb.ToString());
            }

            return formatted;
        }
    }
}
