using System;
using System.Collections.Generic;

namespace DesignPatterns1.CreationalPatterns.Factories
{
    public interface IHotDrink
    {
        void Consume();
    }

    internal class Tea : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("This tea is nice but i'd prefer it with milk.");
        }
    }

    internal class Coffee : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("This coffee is sensational!");
        }
    }

    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }
    
    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"put in tea bag, boil water, pour {amount}, add lemon, enjoy!");
            return new Tea();
        }
    }
    
    internal class CoffeeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"grind some beans, boil water, pour {amount} ml, add cream and sugar, enjoy!");
            return new Coffee();
        }
    }

    public class HotDrinkMachine
    {
        // public enum AvailableDrink
        // {
        //     Coffee, Tea
        // }
        
        // private Dictionary<AvailableDrink, IHotDrinkFactory> factories = new Dictionary<AvailableDrink,IHotDrinkFactory>();
        //
        // public HotDrinkMachine()
        // {
        //     foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
        //     {
        //         var factory = (IHotDrinkFactory) Activator.CreateInstance(
        //             Type.GetType("DesignPatterns1.CreationalPatterns.Factories." +
        //                          Enum.GetName(typeof(AvailableDrink), drink) + "Factory")
        //         );
        //         factories.Add(drink, factory);
        //     }
        // }
        //
        // public IHotDrink MakeDrink(AvailableDrink drink, int amount)
        // {
        //     return factories[drink].Prepare(amount);
        // }
        private List<Tuple<string, IHotDrinkFactory>> factories = new List<Tuple<string, IHotDrinkFactory>>();
        public HotDrinkMachine()
        {
            foreach (var type in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                if (typeof(IHotDrinkFactory).IsAssignableFrom(type) && !type.IsInterface)
                {
                    factories.Add(Tuple.Create(
                        type.Name.Replace("Factory", string.Empty),
                        (IHotDrinkFactory)Activator.CreateInstance(type)
                        ));
                }
            }
        }

        public IHotDrink MakeDrink()
        {
            Console.WriteLine("Available Drinks: ");
            for (var index = 0; index < factories.Count; index++)
            {
                var tuple = factories[index];
                Console.WriteLine($"{index}: {tuple.Item1}");
            }

            while (true)
            {
                string s;
                if ((s = Console.ReadLine()) != null 
                    && int.TryParse(s, out int i)
                    && i >= 0
                    && i < factories.Count)
                {
                    Console.WriteLine("Specify amount: ");
                    s = Console.ReadLine();
                    if (s != null
                        && int.TryParse(s, out int amount)
                        && amount > 0)
                    {
                        return factories[i].Item2.Prepare(amount);
                    }
                }

                Console.WriteLine("Incorrect input, try again!");
            }
        }
    }
    //
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var machine = new HotDrinkMachine();
    //         var drink = machine.MakeDrink();
    //         drink.Consume();
    //     }
    // }
}