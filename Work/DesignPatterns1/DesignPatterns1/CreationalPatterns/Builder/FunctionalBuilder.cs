using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns1.CreationalPatterns.Builder
{
    public class Person2
    {
        public string Name, Position;
    }
    
    public sealed class PersonBuilder
    {
        //sealed class: cannot inherit from it, or extend 
        private readonly List<Func<Person2, Person2>> actions = new List<Func<Person2, Person2>>();

        public PersonBuilder Called(string name) => Do(p => p.Name = name);
        
        public PersonBuilder Do(Action<Person2> action) => AddAction(action);

        public Person2 Build() 
            => actions.Aggregate(new Person2(), (p, f) => f(p));
        private PersonBuilder AddAction(Action<Person2> action)
        {
            actions.Add(p =>
            {
                action(p);
                return p;
            });
            return this;
        }
    }

    public static class PersonBuilderExtensions
    {
        public static PersonBuilder WorksAs
            (this PersonBuilder builder, string position)
            => builder.Do(p => p.Position = position);
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var person = new PersonBuilder()
    //             .Called("Sarah")
    //             .WorksAs("Developer")
    //             .Build();
    //         
    //         Console.WriteLine(person.Name);
    //     }
    // }
}