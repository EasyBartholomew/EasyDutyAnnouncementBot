using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace EasyDutyAnnouncementBot.BL.Models
{
    [DataContract(IsReference = true)]
    public class Platoon
    {
        [IgnoreDataMember]
        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        [IgnoreDataMember]
        public DutyQueue DutyQueue => _dutyQueue;

        public Student CreateStudent(string surname, string name)
        {
            var student = new Student(surname, name);

            if (_students.FirstOrDefault(s => s.Equals(student)) != null)
                throw new ArgumentException("Студент с такими данными уже был создан!");

            _students.Add(student);
            _students.Sort();

            return student;
        }

        public bool RemoveStudent(Student student)
        {
            return _students.RemoveAll(s => s.Equals(student)) != 0;
        }

        //Add after checking on collisions
        public bool RemoveStudent(string privateData)
        {
            var students = _students.Where(s => s.RecognizeSelf(privateData)).ToArray();

            if (students.Length == 0)
                return false;

            if (students.Length > 1)
                throw new Exception(); //Collision exception here

            return this.RemoveStudent(students.First());
        }

        public bool RemoveStudent(string surname, string name)
        {
            var student = _students.FirstOrDefault(s =>
            (s.Surname == surname) && (s.Name == name));

            if (student == null)
                return false;

            return this.RemoveStudent(student);
        }

        public void RemoveAll()
        {
            this._dutyQueue.ExcludeAll();
            this._students.Clear();
        }

        public Platoon()
        {
            _students = new List<Student>();
            _dutyQueue = new DutyQueue(this);
        }

        [DataMember]
        private List<Student> _students;

        [DataMember]
        private DutyQueue _dutyQueue;
    }
}