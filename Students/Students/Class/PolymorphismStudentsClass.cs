using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.Class
{
    public class PolymorphismStudentsClass : EncapsulationStudentsClass
    {
        public override void ShowInfo()
        {
            Console.WriteLine($"Name: {name}, ID: {studentID}, Age: {age} is a master student." );
        }
    }
}
