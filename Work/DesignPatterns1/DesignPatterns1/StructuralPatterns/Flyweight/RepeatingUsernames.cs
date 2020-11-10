using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace DesignPatterns1.StructuralPatterns.Flyweight
{
    public class User9
    {
        private string fullName;
        
        public  User9(string fullName)
        {
            this.fullName = fullName;
        }
    }
    
    public class UserWithFlyWeight
    {
        private static List<string> strings = new List<string>();
        private int[] names;
        
        public  UserWithFlyWeight(string fullName)
        {
            int getOrAdd(string s)
            {
                int idx = strings.IndexOf(s);
                if (idx != -1)
                    return idx;
                else
                {
                    strings.Add(s);
                    return strings.Count - 1;
                }
            }
                       
            names = fullName.Split(' ').Select(getOrAdd).ToArray();
        }

        public string FullName => string.Join(" ", names.Select(i => strings[i]));
    }
    
    // [TestFixture]
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //          
    //     }
    //
    //     [Test]
    //     public void TestUser()
    //     {
    //         var firstNames = Enumerable.Range(0, 100).Select(_ => RandomString());
    //         var lastNames = Enumerable.Range(0, 100).Select(_ => RandomString());
    //
    //         var users = new List<User9>();
    //         
    //         foreach (var firstname in firstNames)
    //             foreach (var lastname in lastNames)
    //                 users.Add(new User9($"{firstname} {lastname}"));
    //
    //         ForceGC();
    //
    //         dotMemory.Check(memory =>
    //         {
    //             Console.WriteLine(memory.SizeInBytes); //1st time: 8,490,891
    //         });
    //     }
    //     
    //     [Test]
    //     public void UserWithFlyWeight()
    //     {
    //         var firstNames = Enumerable.Range(0, 100).Select(_ => RandomString());
    //         var lastNames = Enumerable.Range(0, 100).Select(_ => RandomString());
    //
    //         var users = new List<UserWithFlyWeight>();
    //         
    //         foreach (var firstname in firstNames)
    //         foreach (var lastname in lastNames)
    //             users.Add(new UserWithFlyWeight($"{firstname} {lastname}"));
    //
    //         ForceGC();
    //
    //         dotMemory.Check(memory =>
    //         {
    //             Console.WriteLine(memory.SizeInBytes); // 8,718,401 
    //         });
    //     }
    //
    //     private void ForceGC()
    //     {
    //         GC.Collect();
    //         GC.WaitForPendingFinalizers();
    //         GC.Collect();
    //     }
    //
    //     private string RandomString()
    //     {
    //         Random rand = new Random();
    //
    //         return new string(
    //             Enumerable.Range(0, 10)
    //                 .Select(i => (char)('a' + rand.Next(26)))
    //                 .ToArray()
    //             );
    //     }
    // }
}