using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace EasyDutyAnnouncementBot.BL.Models
{
    [DataContract(IsReference = true)]
    public class DutyQueue : IEnumerable<Student>
    {
        [IgnoreDataMember]
        public IReadOnlyList<Student> DutyStudents => _dutyStudents;

        [IgnoreDataMember]
        public Platoon Platoon => this._platoon;

        [IgnoreDataMember]
        public bool IsEmpty => _dutyStudents.Count == 0;

        public void SetCurrentDuty(string surname)
        {
            var student = _platoon.Students.SingleOrDefault(c => c.RecognizeSelf(surname));

            if (student == null)
                throw new NoInstanceException($"Студент с фамилией \"{surname.Trim()}\" " +
                    $"отсутствует в списке!");

            _dutyStudents.RemoveAll(s => s == student);
            _dutyStudents.Insert(0, student);
        }

        public void Exclude(string surname)
        {
            var student = _platoon.Students.SingleOrDefault(c => c.RecognizeSelf(surname));

            if (student == null)
                throw new NoInstanceException($"Студент с фамилией \"{surname.Trim()}\" " +
                    $"отсутствует в списке!");

            _dutyStudents.RemoveAll(s => s == student);
        }

        public void ExcludeAll()
        {
            this._dutyStudents.Clear();
        }

        public void PushCurrentDuty(string surname, uint count)
        {
            var student = _platoon.Students.SingleOrDefault(s => s.RecognizeSelf(surname));

            if (student == null)
                throw new NoInstanceException($"Студент с фамилией \"{surname.Trim()}\" " +
                    $"отсутствует в списке!");

            while (count != 0)
            {
                _dutyStudents.Insert(0, student);
                count--;
            }
        }

        public void PushCurrentDuty(string surname)
        {
            PushCurrentDuty(surname, 1);
        }

        public void PopCurrentDuty()
        {
            var current = this.GetCurrentDuty();
            this._dutyStudents.Remove(current);
            if (current != GetCurrentDuty())
                this._dutyStudents.Add(current);
        }

        public void ExcludeRanks(StudentRank ranks)
        {
            _dutyStudents.RemoveAll(s => (s.Rank & ranks) != 0);
        }

        public void IncludeRanks(StudentRank ranks)
        {
            var targets = _platoon.Students.Where(s => (s.Rank & ranks) != 0);

            foreach (var target in targets)
            {
                if (DutyStudents.Contains(target))
                    continue;

                _dutyStudents.Add(target);
            }
        }

        public Student AddDuty(string surname)
        {
            var student = _platoon.Students
                .SingleOrDefault(s => s.RecognizeSelf(surname));

            if (student == null)
            {
                if (_platoon.Students.Count == 0)
                    throw new NoInstanceException($"Список студентов пуст!");
                else
                    throw new NoInstanceException($"Студент с фамилией \"{surname.Trim()}\" не был найден!");
            }

            _dutyStudents.Add(student);

            return student;
        }

        public void AddList(IEnumerable<string> privateDatas)
        {
            var exceptionList = new List<string>();

            if (_platoon.Students.Count == 0)
                throw new NoInstanceException($"Список студентов пуст!");

            foreach (var privateData in privateDatas)
            {
                var student = _platoon.Students
                    .SingleOrDefault(s => s.RecognizeSelf(privateData));

                if (student == null)
                {
                    exceptionList.Add(privateData);
                    continue;
                }

                _dutyStudents.Add(student);
            }

            if (exceptionList.Count != 0)
            {
                var exceptionMessage = string.Empty;

                if (exceptionList.Count > 1)
                    exceptionMessage = "Cтуденты с фамилиями: ";
                else
                    exceptionMessage = "Студент с фамилией: ";

                foreach (var surname in exceptionList)
                {
                    exceptionMessage += $"\"{surname.Trim()}\", ";
                }

                throw new NoInstanceException(exceptionMessage
                    .Remove(exceptionMessage.Length - 2, 2) + " не был найден!");
            }
        }

        public void SetList(IEnumerable<string> surnames)
        {
            _dutyStudents = new List<Student>();

            this.AddList(surnames);
        }

        public Student GetCurrentDuty()
        {
            var currentDuty = DutyStudents.FirstOrDefault();

            return currentDuty ??
                throw new NoInstanceException("В очереди на дежурство нет ни одного студента!");
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return DutyStudents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal DutyQueue(Platoon platoon, bool pushAllStudents = false)
        {
            _platoon = platoon;
            _dutyStudents = new List<Student>();

            if (pushAllStudents)
                _dutyStudents.AddRange(_platoon.Students);
        }

        [DataMember]
        private List<Student> _dutyStudents;

        [DataMember]
        private readonly Platoon _platoon;
    }
}
