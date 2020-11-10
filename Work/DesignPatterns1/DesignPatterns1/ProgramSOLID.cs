// using System;
// using System.Diagnostics;
// using DesignPatterns1.SOLID;
//
// namespace DesignPatterns1
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             //singleResponsibility:
//             
//             // var j = new Journal();
//             // j.AddEntry("I cried today");
//             // j.AddEntry("I ate a bug");
//             //
//             // Console.WriteLine(j);
//             //
//             // var p = new Persistence();
//             // var filename = @"c:\temp\journal.txt";
//             // p.SaveToFile(j, filename, true);
//             
//             //open-Closed:
//             
//             // var apple = new Product("Apple", Color.Green, Size.Small);
//             // var tree = new Product("Tree", Color.Green, Size.Large);
//             // var house = new Product("House", Color.Blue, Size.Large);
//             //
//             // Product[] products = {apple, tree, house};
//             //
//             // var pf = new ProductFilter();
//             //
//             // Console.WriteLine("Green products (old):");
//             // foreach (var p in pf.FilterByColor(products, Color.Green))
//             // {
//             //     Console.WriteLine($" - {p.Name} is green");
//             // }
//             //
//             // var bf = new BetterFilter();
//             // Console.WriteLine("Green products (new):");
//             // foreach (var p in bf.Filter(products, new ColorSpecification(Color.Green)))
//             // {
//             //     Console.WriteLine($" - {p.Name} is green");
//             // }
//             //
//             // Console.WriteLine("Large blue items");
//             // foreach (var p in bf.Filter(products, new AndSpecification<Product>(new SizeSpecification(Size.Large), new ColorSpecification(Color.Blue))))
//             // {
//             //     Console.WriteLine($" - {p.Name} is large and blue");
//             // }
//             
//             // liskov substitution:
//             
//             // Rectangle rc = new Rectangle(2, 3);
//             // Console.WriteLine($"{rc} has area {Area(rc)}");
//             //
//             // Rectangle sq = new Square(); 
//             // //according to the liskov substitution princple, you should be able to change this to a rectangle and nothing should go wrong.
//             // //However we'd now have a width of 4 and a height of 0 (default int)
//             // //You should be able to "upcast" to the base type, and be okay. But the way this is done here violates that 
//             // sq.Width = 4;
//             // Console.WriteLine($"{sq} has area {Area(sq)}");
//             
//             // interface segregation: 
//             
//             // dependency inversion: 
//
//             var parent = new Person {Name = "John"};
//             var child1 = new Person {Name = "Chris"};
//             var child2 = new Person {Name = "Mary"};
//             
//             var relationships = new Relationships();
//             relationships.AddParentAndChild(parent, child1);
//             relationships.AddParentAndChild(parent, child2);
//
//             // var children = relationships.FindAllChildrenOf("John");
//             //
//             // foreach (var c in children)
//             // {
//             //     Console.WriteLine(c.Name);
//             // }
//             
//             var research = new Research(relationships);
//         }
//         
//         static public int Area(Rectangle r) => r.Width * r.Height;
//     }
// }