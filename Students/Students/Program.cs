using Students.Class;
using System.Xml.Linq;

namespace Students
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EncapsulationStudentsClass student = new EncapsulationStudentsClass();
            student.SetName("Chopper");
            student.studentID = 1138;
            student.age = 29;
            student.ShowInfo();
            Console.WriteLine($"----------------------");

            InheritanceStudensClass studentMaster = new InheritanceStudensClass();
            studentMaster.SetName("Nest");
            studentMaster.DoResearch();
            Console.WriteLine($"----------------------");

            PolymorphismStudentsClass studentPoly = new PolymorphismStudentsClass();
            studentPoly.SetName("Nest");
            studentPoly.studentID = 1138;
            studentPoly.age = 30;
            studentPoly.ShowInfo();
            Console.WriteLine($"----------------------");

            List<AbstractionStudentsClass> students = new List<AbstractionStudentsClass>
            {
                new BachelorStudent { Name = "Chopper" },
                new MasterStudent { Name = "Nest" }
            };

            foreach (var s in students)
            {
                s.Learn();
            }
        }
    }
}
