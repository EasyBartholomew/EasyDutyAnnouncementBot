using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;


namespace EasyDutyAnnouncementBot.BL.Models
{
    [DataContract(IsReference = true)]
    public class Platoon : IEnumerable<Student>
    {
        [IgnoreDataMember]
        public CustomQueue DutyQueue => _dutyQueue;

        [IgnoreDataMember]
        public CustomQueue ReportQueue => _reportQueue;

        [DataMember]
        public DayOfWeek TagetDay { get; set; }

        public Student CreateStudent(string surname, string name)
        {
            var student = new Student(surname, name);

            if (_students.FirstOrDefault(s => s.Equals(student)) != null)
                throw new InvalidOperationException("Студент с такими данными уже был создан!");
            if (_students.Count >= StudentsMaxCount)
                throw new InvalidOperationException("Достигнуто максимальное количество студентов во взводе!");

            _students.Add(student);
            _students.Sort();

            return student;
        }

        public bool Contains(Student student)
        {
            return _students.Contains(student);
        }

        public bool Contains(string privateData)
        {
            return _students.FirstOrDefault(s => s.RecognizeSelf(privateData)) != null;
        }

        public Student TryGetStudent(string privateData)
        {
            return null;
        }

        public Student[] GetStudents(string privateData)
        {
            return _students.Where(s => s.RecognizeSelf(privateData)).ToArray();
        }

        public Student GetStudent(string privateData)
        {
            var students = _students.Where(s => s.RecognizeSelf(privateData)).ToArray();


            if (students.Length > 1)
            {
                var exceptionMessage = new StringBuilder("По введённым данным невозможно однозначно определить студента!\n" +
                    "Кого именно имели в виду?\n");

                foreach (var student in students)
                {
                    exceptionMessage.Append($"{student},\n");
                }

                exceptionMessage
                    .Remove(exceptionMessage.Length - 2, 2)
                    .Append('.');

                throw new CollisionException(exceptionMessage.ToString());
            }


            if (students.Length == 0)
                throw new NoInstanceException($"\"{privateData}\" отсутствует в списке студентов!");

            return students.First();
        }

        public bool RemoveStudent(Student student)
        {
            try
            {
                _dutyQueue.Exclude(student.ToString());
                _reportQueue.Exclude(student.ToString());
            }
            catch (Exception)
            { }

            return _students.RemoveAll(s => s.Equals(student)) != 0;
        }

        public bool RemoveStudent(string privateData)
        {
            var students = _students.Where(s => s.RecognizeSelf(privateData)).ToArray();

            if (students.Length == 0)
                return false;

            if (students.Length > 1)
                throw new CollisionException("По введённым данным невозможно однозначно определить студента!\n" +
                    "Попробуйте ввести имя и фамилию)");

            return this.RemoveStudent(students.First());
        }

        public void RemoveAll()
        {
            this._dutyQueue.ExcludeAll();
            this._students.Clear();
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return this._students.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Platoon()
        {
            _students = new List<Student>();
            _dutyQueue = new CustomQueue(this, StudentsMaxCount);
            _reportQueue = new CustomQueue(this, StudentsMaxCount); //Change latter
            TagetDay = DayOfWeek.Thursday;  //Change latter
        }

        public static Int32 StudentsMaxCount { get; } = 30;


        [DataMember]
        private readonly List<Student> _students;

        [DataMember]
        private readonly CustomQueue _dutyQueue;

        [DataMember]
        private readonly CustomQueue _reportQueue;
    }
}