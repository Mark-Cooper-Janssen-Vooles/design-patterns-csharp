using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using MoreLinq;
using NUnit.Framework;


namespace DesignPatterns1.CreationalPatterns.Singleton
{
    public interface IDatabase
    {
        int GetPopulation(string name);
    }

    public class SingletonDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;
        private static int instanceCount;
        public static int Count => instanceCount;

        private SingletonDatabase()
        {
            instanceCount++;
            Console.WriteLine("Initializing Database");
            
            capitals = File.ReadAllLines(@"C:\Users\mark.janssen-vooles\code\tutorials\DesignPatternsC#\Work\DesignPatterns1\DesignPatterns1\CreationalPatterns\Singleton\Capitals.txt")
                .Batch(2)
                .ToDictionary(
                list => list.ElementAt(0).Trim(),
                list => int.Parse(list.ElementAt(1))
                );
        }
        public int GetPopulation(string name)
        {
            return capitals[name];
        }
        
        //private static SingletonDatabase instance = new SingletonDatabase();
        private static Lazy<SingletonDatabase> instance = 
            new Lazy<SingletonDatabase>(() => new SingletonDatabase());
        // to actually get the instance, you have to call the static instance method.
        // There is no way to have more than one instance, it will only refer to the one reference above.
        //public static SingletonDatabase Instance => instance;
        public static SingletonDatabase Instance => instance.Value;
    }

    public class SingletonRecordFinder
    {
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += SingletonDatabase.Instance.GetPopulation(name);
            
            return result;
        }
    }

    public class ConfigurableRecordFinder
    {
        private IDatabase database;

        public ConfigurableRecordFinder(IDatabase database)
        {
            this.database = database;
        }
        
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += database.GetPopulation(name);
            
            return result;
        }
    }

    public class OrdinaryDatabase : IDatabase //not a singleton, but we can use it as one provided we use a DI container
    {
        private Dictionary<string, int> capitals;

        public OrdinaryDatabase()
        {
            Console.WriteLine("Initializing Database");
            
            capitals = File.ReadAllLines(@"C:\Users\mark.janssen-vooles\code\tutorials\DesignPatternsC#\Work\DesignPatterns1\DesignPatterns1\CreationalPatterns\Singleton\Capitals.txt")
                .Batch(2)
                .ToDictionary(
                    list => list.ElementAt(0).Trim(),
                    list => int.Parse(list.ElementAt(1))
                );
        }
        public int GetPopulation(string name)
        {
            return capitals[name];
        }
    }

    public class MockDatabase : IDatabase
    {
        public int GetPopulation(string name)
        {
            return new Dictionary<string, int>
            {
                ["alpha"] = 1,
                ["beta"] = 2,
                ["gamma"] = 3
            }[name];
        }
    }

    [TestFixture]
    public class SingletonTests
    {
        [Test]
        public void DIPopulationTest()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<OrdinaryDatabase>()
                .As<IDatabase>()
                .SingleInstance(); //this is how you tell the DI Container that its a singleton
            cb.RegisterType<ConfigurableRecordFinder>();
            using (var c = cb.Build())
            {
                var rf = c.Resolve<ConfigurableRecordFinder>();
            }
        }
        
        [Test]
        public void IsSingletonTest()
        {
            var db = SingletonDatabase.Instance;
            var db2 = SingletonDatabase.Instance;

            Assert.That(db, Is.SameAs(db2));
            Assert.That(SingletonDatabase.Count, Is.EqualTo(1));
        }

        [Test]
        public void SingletonPopulationTest()
        {
            var recordFinder = new SingletonRecordFinder();
            var names = new[] {"Seoul", "Mexico City"};
            int tp = recordFinder.GetTotalPopulation(names);

            Assert.That(tp, Is.EqualTo(3490));
        }
        
        [Test]
        public void ConfigurablePopulationTest()
        {
            var recordFinder = new ConfigurableRecordFinder(new MockDatabase());
            var names = new[] {"alpha", "beta"};
            int tp = recordFinder.GetTotalPopulation(names);

            Assert.That(tp, Is.EqualTo(3));
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var db = SingletonDatabase.Instance;
    //         var city = "Tokyo";
    //         Console.WriteLine(db.GetPopulation(city));
    //     }
    // }
}