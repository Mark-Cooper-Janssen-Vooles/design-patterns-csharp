using System;

namespace DesignPatterns1.StructuralPatterns.Decorator
{
    public interface ICreature
    {
        int Age { get; set; }
    }

    public interface IBird2 : ICreature
    {
        public void Fly()
        {
            if (Age >= 10)
                Console.WriteLine("I am flying");
        }
    }

    public interface ILizard2 : ICreature
    {
        public void Crawl()
        {
            if (Age >= 10)
                Console.WriteLine("I am crawling");
        }
    }

    public class Organism {}

    public class Dragon2 : Organism, IBird2, ILizard2
    {
        public int Age { get; set; }
    }
    
    // inheritance (can't inherit from multiple classes, i.e. already inherits from organism)
    // SmartDragon(Dragon) - typical decorator design pattern 
    // if u just want to add behaviour: 
    // extension methods can add behavior - option 1 
    // c# 8 default interface methods - option 2
    
    //  class Program
    //  {
    //      static void Main(string[] args)
    //      {
    //          var d = new Dragon2 {Age = 5};
    //          // we don't have access to Fly or Crawl, the methods are on the interfaces not the class
    //          
    //          ((ILizard2)d).Crawl(); //if we cast it, we have access it
    //          
    //          if (d is IBird2 bird)
    //              bird.Fly(); //can also do this 
    //          
    //          //to do this method you need the casts!
    //          
    //          //this is more intrusive than adding extension methods: extension methods don't require you edit the class itself
    //      }
    // }
}