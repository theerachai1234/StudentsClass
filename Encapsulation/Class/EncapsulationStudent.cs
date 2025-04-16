using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encapsulation.Class
{
    public class EncapsulationStudent
    {
        private string name;
        private int studentID;
        private int age;

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetStudentID(int studentID)
        {
            this.studentID = studentID;
        }

        public int GetStudentID()
        {
            return this.studentID;
        }

        public void SetAge(int age)
        {
            this.age = age;
        }

        public int getAge()
        {
            return this.age;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Name: {name}, ID: {studentID}, Age: {age}");
        }

    }
}
