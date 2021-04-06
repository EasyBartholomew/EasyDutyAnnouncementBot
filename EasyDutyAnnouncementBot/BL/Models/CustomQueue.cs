using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace EasyDutyAnnouncementBot.BL.Models
{
    [DataContract(IsReference = true)]
    public class CustomQueue : IEnumerable<Student>
    {
        [IgnoreDataMember]
        public Platoon Platoon => this._platoon;

        [IgnoreDataMember]
        public bool IsEmpty => _students.Count == 0;

        [IgnoreDataMember]
        public int Capacity => this._capacity;

        public void SetCurrent(string privateData)
        {
            var student = _platoon.GetStudent(privateData);

            if (student == null)
                throw new NoInstanceException($"Студент \"{privateData.Trim()}\" " +
                    $"отсутствует в списке!");

            if (_students.Count >= this.Capacity)
                throw new InvalidOperationException("В настоящее время в очереди максимальное количество человек!");

            _students.RemoveAll(s => s == student);
            _students.Insert(0, student);
        }

        public void SetList(IEnumerable<string> privateDatas)
        {
            _students = new List<Student>();

            this.AddList(privateDatas);
        }

        public void Exclude(string privateData)
        {
            var student = _platoon.GetStudent(privateData);

            if (student == null)
                throw new NoInstanceException($"Студент \"{privateData.Trim()}\" " +
                    $"отсутствует в списке!");

            _students.RemoveAll(s => s == student);
        }

        public void ExcludeAll()
        {
            this._students.Clear();
        }

        public void PushCurrent(string privateData, uint count)
        {
            var student = _platoon.GetStudent(privateData);

            if (student == null)
                throw new NoInstanceException($"Студент \"{privateData.Trim()}\" " +
                    $"отсутствует в списке!");

            if (_students.Count >= this.Capacity)
                throw new InvalidOperationException("В настоящее время в очереди максимальное количество человек!");

            while (count != 0)
            {
                _students.Insert(0, student);
                count--;
            }
        }

        public void PushCurrent(string privateData)
        {
            PushCurrent(privateData, 1);
        }

        public void PopCurrent()
        {
            var current = this.GetCurrentDuty();
            this._students.Remove(current);
            if (current != GetCurrentDuty())
                this._students.Add(current);
        }

        public void ExcludeRanks(StudentRank ranks)
        {
            _students.RemoveAll(s => (s.Rank & ranks) != 0);
        }

        public void IncludeRanks(StudentRank ranks)
        {
            var targets = _platoon.Where(s => (s.Rank & ranks) != 0);

            foreach (var target in targets)
            {
                if (_students.Contains(target))
                    continue;

                _students.Add(target);
            }
        }

        public Student Add(string privateData)
        {
            var student = _platoon.GetStudent(privateData);


            if (student == null)
            {
                if (_platoon.Count() == 0)
                    throw new NoInstanceException($"Список студентов пуст!");
                else
                    throw new NoInstanceException($"Студент \"{privateData.Trim()}\" не был найден!");
            }

            if (_students.Count >= this.Capacity)
                throw new InvalidOperationException("В настоящее время в очереди максимальное количество человек!");

            _students.Add(student);

            return student;
        }

        public void AddList(IEnumerable<string> privateDatas)
        {
            var exceptionList = new List<string>();

            if (_platoon.Count() == 0)
                throw new NoInstanceException($"Список студентов пуст!");

            if (_students.Count >= this.Capacity)
                throw new InvalidOperationException("В настоящее время в очереди максимальное количество человек!");

            if ((_students.Count + privateDatas.Count()) > this.Capacity)
                throw new InvalidOperationException(
                    "Невозможно добавить всех студентов из списка во избежание превышения лимита очереди!");

            if (privateDatas.Count() == 1)
            {
                this.Add(privateDatas.First());
                return;
            }

            //Check
            foreach (var privateData in privateDatas)
            {
                try
                {
                    var student = _platoon.GetStudent(privateData);
                    _students.Add(student);
                }
                catch (NoInstanceException)
                {
                    exceptionList.Add(privateData);
                    continue;
                }
            }

            if (exceptionList.Count != 0)
            {
                var exceptionMessage = string.Empty;

                foreach (var surname in exceptionList)
                {
                    exceptionMessage += $"\"{surname.Trim()}\", ";
                }

                exceptionMessage = exceptionMessage.Remove(exceptionMessage.Length - 2, 2);

                if (exceptionList.Count > 1)
                    exceptionMessage = exceptionMessage.Insert(0, "Cтуденты: ") + " не были найдены!";
                else
                    exceptionMessage = exceptionMessage.Insert(0, "Студент: ") + " не был найден!";

                throw new NoInstanceException(exceptionMessage);
            }
        }

        public Student GetCurrentDuty()
        {
            var currentDuty = _students.FirstOrDefault();

            return currentDuty ??
                throw new NoInstanceException("В очереди на дежурство нет ни одного студента!");
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return _students.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal CustomQueue(Platoon platoon, Int32 capacity, bool pushAllStudents = false)
        {
            _platoon = platoon;
            _capacity = capacity;
            _students = new List<Student>();

            if (pushAllStudents)
                _students.AddRange(_platoon);
        }

        [DataMember]
        private List<Student> _students;

        [DataMember(IsRequired = false)]
        private readonly int _capacity;

        [DataMember]
        private readonly Platoon _platoon;
    }
}
