using System;
using System.Collections.Generic;

namespace DesignPatterns1.CreationalPatterns.Factories
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
  
    public interface IPersonFactory
    {
        public Person CreatePerson(string name);
    }

    public class PersonFactory : IPersonFactory
    {
        private int _personCount = 0;
        public Person CreatePerson(string name)
        {
            var person = new Person();
            person.Name = name;
            person.Id = _personCount;
            _personCount++;
            
            return person;
        }
    }
}