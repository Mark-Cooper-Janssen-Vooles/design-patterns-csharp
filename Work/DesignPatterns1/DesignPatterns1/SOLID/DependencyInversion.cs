using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DesignPatterns1.SOLID
{
    public enum Relationship
    {
        Parent, 
        Child,
        Sibling
    }

    public class Person
    {
        public string Name;
        //public DateTime DateOfBirth;
    }

    public interface IRelationshipBrowser
    {
        IEnumerable<Person> FindAllChildrenOf(string name);
    }
    
    //low-level
    public class Relationships : IRelationshipBrowser
    {
        private List<(Person, Relationship, Person)> relations = new List<(Person, Relationship, Person)>();

        public void AddParentAndChild(Person parent, Person child)
        {
            relations.Add((parent, Relationship.Parent, child));
            relations.Add((child, Relationship.Child, parent));
        }
        
        public IEnumerable<Person> FindAllChildrenOf(string name)
        {
            var allChildren = new List<Person>();
            foreach (var r in relations.Where(x =>
                x.Item1.Name == name && x.Item2 == Relationship.Parent))
            {
                allChildren.Add(r.Item3);
            }

            return allChildren;
        }
    }

    public class Research
    {
        public Research(IRelationshipBrowser browser)
        {
            foreach (var p in browser.FindAllChildrenOf("John"))
            {
                Console.WriteLine($"John has a child called {p.Name}");
            }
        }
    }
}