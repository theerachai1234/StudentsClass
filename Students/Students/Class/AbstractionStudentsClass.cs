using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students.Class
{
    public abstract class AbstractionStudentsClass
    {
        public string Name { get; set; }
        public abstract void Learn();
    }

    public class BachelorStudent : AbstractionStudentsClass
    {
        public override void Learn()
        {
            Console.WriteLine($"{Name} is a bachelor student.");
        }
    }

    public class MasterStudent : AbstractionStudentsClass
    {
        public override void Learn()
        {
            Console.WriteLine($"{Name} is a master student.");
        }
    }
}
