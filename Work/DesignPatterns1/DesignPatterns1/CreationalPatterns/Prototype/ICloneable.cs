using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace DesignPatterns1.CreationalPatterns.Prototype
{
    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            //works on any object
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, self);
            stream.Seek(0, SeekOrigin.Begin);
            object copy = formatter.Deserialize(stream);
            stream.Close();
            return (T)copy;
        }

        public static T DeepCopyXml<T>(this T self)
        {
            using (var ms = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(ms, self);
                ms.Position = 0;
                return (T)s.Deserialize(ms);
            }
        }
    }
    
    //[Serializable] (need this if using binary serialisation)
    public class Person
    {
        public string[] Names;
        public Address Address;

        public Person() //need this empty constructor for xml serialization
        {
        }
        
        public Person(string[] names, Address address)
        {
            this.Names = names;
            this.Address = address;
        }

        public override string ToString()
        {
            return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
        }

        //ICloneable implementation: (bad)
        public object Clone()
        {
            return new Person(Names, (Address)Address.Clone());
        }

        //copy constructor: 
        public Person(Person other)
        {
            Names = new [] {other.Names[0], other.Names[1]};
            Address = new Address(other.Address);
        }
        
        //deep copy interface:
        // public Person DeepCopy()
        // {
        //     return new Person(Names, Address.DeepCopy());
        // }
    }

    //[Serializable] (need this if using binary serialization)
    public class Address
    {
        public string StreetName;
        public int HouseNumber;

        public Address() //need this empty constructor for xml serialization
        {
        }

        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }
        

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }
        
        public object Clone()
        {
            return new Address(StreetName, HouseNumber);
        }

        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }
        
        // public Address DeepCopy()
        // {
        //     return new Address(StreetName, HouseNumber);
        // }
    }

    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var john = new Person(
    //             new []{"John", "Smith"},
    //             new Address("London Road", 123)
    //             );
    //         //var jane = john;
    //         //jane.Names[0] = "Jane"; //causes both to say Jane, because you've just modified the reference. Not a "deep copy" 
    //
    //         //IClonable:
    //         //var jane = (Person)john.Clone();
    //         //jane.Names[0] = "Jane";
    //         //jane.Address.StreetName = "Fake street"; //the problem is the address object was not deep cloned! it changes john's address.
    //
    //         //copy Constructors:
    //         //var jane = new Person(john);
    //         //.Names[0] = "Jane";
    //         //jane.Address.HouseNumber = 321;
    //         
    //         // Explicit deep copy interface
    //         // var jane = john.DeepCopy();
    //         // jane.Address.HouseNumber = 321;
    //         
    //         //Serialization: 
    //         var jane = john.DeepCopyXml();
    //         jane.Names[0] = "Jane";
    //         jane.Address.HouseNumber = 321;
    //         
    //         Console.WriteLine(john);
    //         Console.WriteLine(jane);
    //     }
    // }
}