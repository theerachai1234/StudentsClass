using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Students.Class
{
    public class InheritanceStudensClass : EncapsulationStudentsClass
    {
        public void DoResearch()
        {
            Console.WriteLine($"{name} is a master student.");
        }
    }
}
