using System;

namespace DesignPatterns1.CreationalPatterns.Singleton
{
    public class CEO
    {
        private static string name;
        private static int age;

        public string Name
        {
            get => name;
            set => name = value;
        }
        
        public int Age
        {
            get => age;
            set => age = value;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var ceo = new CEO();
    //         ceo.Name = "Adam Smith";
    //         ceo.Age = 55;
    //         
    //         var ceo2 = new CEO();
    //         
    //         Console.WriteLine(ceo2); //because the name and age are static, they refer to the same object
    //     }
    // }
}