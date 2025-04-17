using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.Class
{
    public class EncapsulationStudentsClass
    {
        public string name;
        public int studentID { get; set; }
        public int age { get; set; }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Name: {name}, ID: {studentID}, Age: {age}");
        }
    }
}
