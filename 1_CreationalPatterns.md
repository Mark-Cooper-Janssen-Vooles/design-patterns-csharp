Contents:
- Builder
- Factories
- Prototype
- Singleton


# Builder 
- Contents:
  - Overview 
  - An example without a custom builder (StringBuilder is a builder)
  - An example with a builder
  - A fluent builder
  - Fluent builder inheritance => don't do it!
  - A functional builder
  - A faceted builder


- Motivation 
  - Some objects are simple and can be created in a single constructor call
  - Other objects require a lot of ceremony to create (construct an object piece by piece)
  - Having an object with 10 constructor arguments is not productive
  - Instead, opt for piecewise construction (piece-by-piece) => this is the builder
  - Builder provides an API for constructing an object step-by-step


- Builder: When piecewise object construction is complicated, provide an API for doing it succinctly.


- Life without builder:
````c#
public static void Example()
{
    var hello = "hello";
    var sb = new StringBuilder();
    sb.Append("<p>");
    sb.Append(hello);
    sb.Append("</p>");

    Console.WriteLine(sb);

    var words = new[] {"hello", "world"};
    sb.Clear();
    sb.Append("<ul>");
    foreach (var word in words)
    {
        sb.AppendFormat($"<li>{word}</li>");
    }

    Console.WriteLine(sb);
}

//outputs:
//<ul><li>hello</li><li>world</li>
````
- With builder:
- the "HtmlBuilder" has a sole purpose in life to build up html elements
- Essentially a HtmlBuilder is made for each element, which you specifiy in the argument
- You can then add children to this, and you pass the element and the string you want to be in that element, which adds a new HtmlElement
- You can then call toString on this (auto called via console.WriteLine), and it will use the override on the htmlElement itself to make the output pretty  
- An OOP way to "build" an object one at a time!
````c#
//being called in main: 
public static void ExampleWithBuilder()
{
    var builder = new HtmlBuilder("ul");
    builder.AddChild("li", "hello");
    builder.AddChild("li", "world");

    Console.WriteLine(builder);
}

public class HtmlElement
{
    public string Name, Text;
    public List<HtmlElement> Elements = new List<HtmlElement>();
    private const int indentSize = 2;

    public HtmlElement()
    {
        
    }

    public HtmlElement(string name, string text)
    {
        Name = name;
        Text = text;
    }

    private string ToStringImpl(int indent)
    {
        var sb = new StringBuilder();
        var i = new string(' ', indentSize * indent);
        sb.Append($"{i}<{Name}>\n");

        if (!string.IsNullOrWhiteSpace(Text))
        {
            sb.Append(new string(' ', indentSize * (indent + 1)));
            sb.AppendLine(Text);
        }

        foreach (var e in Elements)
        {
            sb.Append(e.ToStringImpl(indent + 1));
        }
        
        sb.Append($"{i}</{Name}>\n");
        return sb.ToString();
    }

    public override string ToString()
    {
        return ToStringImpl(0);
    }
}

public class HtmlBuilder
{
    private readonly string _rootName;
    HtmlElement root = new HtmlElement();

    public HtmlBuilder(string rootName)
    {
        _rootName = rootName;
        root.Name = rootName;
    }

    public void AddChild(string childName, string childText)
    {
        var e = new HtmlElement(childName, childText);
        root.Elements.Add(e);
    }

    public override string ToString()
    {
        return root.ToString();
    }

    public void Clear()
    {
        root = new HtmlElement {Name = _rootName};
    }
}

//outputs:
// <ul>
//   <li>
//     hello
//   </li>
//   <li>
//     world
//   </li>
// </ul>
````


- Fluent Builder:
  - in a "new StringBuilder()", you can see the .Append returns a stringBuilder itself, allowing you to chain methods together!
````c#
//to chain methods, in our HtmlBuilder, instead of this code: 
public void AddChild(string childName, string childText)
{
    var e = new HtmlElement(childName, childText);
    root.Elements.Add(e);
}
//we change it to this: 
public HtmlBuilder AddChild(string childName, string childText)
{
    var e = new HtmlElement(childName, childText);
    root.Elements.Add(e);
    return this;
}

//which allows this instead: 
public static void ExampleWithBuilder()
{
    var builder = new HtmlBuilder("ul");
    //builder.AddChild("li", "hello");
    //builder.AddChild("li", "world");
    builder.AddChild("li", "hello").AddChild("li", "world");

    Console.WriteLine(builder);
}
````
- Called a "fluid interface" in OOP


- What happens when builders inherit from other builders:
  - if you're using the fluent interface approach things can get bad
  - no easy way to mitigate the inheritance of fluent interfaces, but you can via the use of 'recursive generics'
  - nothing wrong with fluent interface approach until we start inheriting from it...
````c#
public class Person
{
    public string Name;
    public string Position;

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
    }
}

public class PersonInfoBuilder
{
    protected Person person = new Person();

    public PersonInfoBuilder called(string name)
    {
        person.Name = name;
        return this;
    }
}

public class PersonJobBuilder : PersonInfoBuilder
{
    public PersonJobBuilder WorksAsA(string position)
    {
        person.Position = position;
        return this;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var builder = new PersonJobBuilder();
        builder.called("Mark").Works //you can't call WorksAsA on this method, because after you used called() you're returned a PersonInfoBuilder
    }
}
````
- There is a way around the above issue, but it is a terrible idea lol. Just don't inherit from fluent builders!


- Builder Design pattern in a more functional way, "Functional Builder" 
- Note: this is a more functional way to do this, not OOP. There is an example in the code.


- Faceted Builder: 
  - when a single builder isn't enough
  - several builders build up several different aspects
  - you also need a facade (another design pattern)
````c#
public class Person3
{
    //address
    public string StreetAddress, Postcode, City; //might want a builder just for the address
    //employement info
    public string CompanyName, Position;
    public int AnnualIncome;
    
    public override string ToString()
    {
        return $"{nameof(StreetAddress)}: {StreetAddress}, {nameof(Postcode)}: {Postcode}, {nameof(City)}: {City}, " +
                $"{nameof(CompanyName)}: {CompanyName}, {nameof(Position)}: {Position}, {nameof(AnnualIncome)}: {AnnualIncome}";
    }
}

//a facet
public class PersonBuilder3 //facade
{
    //reference type. If you have a value type you could be in trouble, so always work with objects 
    protected Person3 person = new Person3();
    
    //expose personJobBuilder3
    public PersonJobBuilder3 Works => new PersonJobBuilder3(person);
    public PersonAddressBuilder3 Lives => new PersonAddressBuilder3(person);

    //this function allows you to return a person when implicitly called with "Person3 person = new PersonBuilder3();"
    //if you use "var person = new PersonBuilder3();" it will just return  a PersonBuilder3.
    public static implicit operator Person3(PersonBuilder3 pb)
    {
        return pb.person;
    }
}

public class PersonJobBuilder3 : PersonBuilder3
{
    //building up job information on a person3 object
    public PersonJobBuilder3(Person3 person)
    {
        this.person = base.person;
    }

    public PersonJobBuilder3 At(string companyName)
    {
        person.CompanyName = companyName;
        return this;
    }

    public PersonJobBuilder3 AsA(string position)
    {
        person.Position = position;
        return this;
    }

    public PersonJobBuilder3 Earning(int amount)
    {
        person.AnnualIncome = amount;
        return this;
    }
}

public class PersonAddressBuilder3 : PersonBuilder3
{
    public PersonAddressBuilder3(Person3 person)
    {
        this.person = base.person;
    }

    public PersonAddressBuilder3 StreetAddress(string address)
    {
        person.StreetAddress = address;
        return this;
    }

    public PersonAddressBuilder3 Postcode(string postcode)
    {
        person.Postcode = postcode;
        return this;
    }

    public PersonAddressBuilder3 City(string city)
    {
        person.City = city;
        return this;
    }
}


class Program
{
    static void Main(string[] args)
    {
        var pb = new PersonBuilder3();
        Person3 person = pb
            .Works
                .At("Xero")
                .AsA("Developer")
                .Earning(99999)
            .Lives
                .StreetAddress("123 Fake Street")
                .Postcode("1234")
                .City("Melbourne");
        Console.WriteLine(person);
    }
}
````


- A builder is a separate component for building an object
- Can either give builder a constructor or return it via a static function
- To make a builder fluent, return this
- Different facets of an object can built using several builders working together in tandem via a base class


---


# Factories
- Contents:
  - Point Example
  - Factory Method
  - Asynchronous Factory Method
  - Factory
  - Inner Factory
  - Abstract Factory
  - Abstract Factory and OCP
  - Summary


- Covers Factory Method pattern and Abstract Factory pattern
- Factory method very common, abstract factory rate
- Motivation:
  - Object creation logic becomes too convoluted
  - Constructor is not descriptive 
    - Name mandated by name of containing type
    - Cannot overload with same sets of arguments with different names
    - Can turn into 'optional parameter hell' - optional arguments causes confusion over order of arguments etc
  - Object creation (non-piecewise, unlike Builder) can be outsourced to:
    - A separate function (Factory Method)
    - A separate class (Factory)
    - Can create hierarchy of factories with Abstract Factory


- Factory: A component responsible solely for the wholesale (not piecewise) creation of objects 


#### Life Without Factory Method
- This is without using a factory method. If you want to have two constructors but they have the same arguments, you can't. You have to add a third argument, reuse the first constructor and have a switch statement. It needs comments etc so the other devs know whats going on. Gets ugly. I.e.: 
````c#
public enum CoordinateSystem
{
    Cartesian, 
    Polar
}

public class Point
{
    private double x, y;

    //if its polar there is no indication that a is rho and b is theta, so you will need to add XML comments...
    //adds additional mental complexity
    /// <summary>
    /// Initializes a point from EITHER cartesian or polar
    /// </summary>
    /// <param name="a">x if cartesian, rho if polar</param>
    /// <param name="b">y if carterisan, theta if polar</param>
    /// <param name="system"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Point(double a, double b, CoordinateSystem system = CoordinateSystem.Cartesian) //cartesian by default 
    {
        switch (system)
        {
            case CoordinateSystem.Cartesian:
                x = a;
                y = b;
                break;
            case CoordinateSystem.Polar:
                x = a * Math.Cos(b);
                y = a * Math.Sin(b);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
````


#### Life With Factory Method
- You could also use inheritance etc for a different point, but an easier way is to use the Factory Method. 
- You can use resharper to do this! right click the constructor method name => select refactor => "refactor this" => "replace constructor with factory method"
  - The constructor we had is still there, but its private now. Only for intrernal use!
  - Theres a new public static method (the factory method) which invokes the constructor
  - The name of the factory method is not tied to the class like a constructor. 
- The advantages of a factory method: 
  - You get to have an overload with the same set of arguments 
  - The names of the factory methods are unique, so you know what you're creating
````c#
public class Point2
{
    public static Point2 NewCartesianPoint(double x, double y)
    {
        return new Point2(x, y);
    }
    
    public static Point2 NewPolarPoint(double rho, double theta)
    {
        return new Point2(rho * Math.Cos(theta), rho * Math.Sin(theta));
    }

    private double x, y;

    private Point2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        var point = Point2.NewPolarPoint(1, Math.PI / 2);
        Console.WriteLine(point);
    }
}
````


#### Asynchronous Factory Method
- Asynchronous invokation cannot happen everywhere
- Can happen in methods, but not in constructors. 
- What to do? Use a factory / factory method! 
- If the class Foo has to do something asynchronous in the constructor, i.e. load a web page, but you don't use the factory method:
````c#
public class Foo
{
    public Foo()
    {
        
    }

    public async Task<Foo> InitAsync()
    {
        await Task.Delay(1000);
        return this;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var foo = new Foo();
        await foo.InitAsync(); //if you forget this, your object will not be initialised correctly
    }
}
````
- A factory method will inhibit the use of the constructor, meaning there will be no change of people forgetting to call the InitAsync() method
````c#
public class Foo
{
private Foo()
{   
}

private async Task<Foo> InitAsync()
{
    await Task.Delay(1000);
    return this;
}

public static Task<Foo> CreateAsync()
{
    var result = new Foo();
    return result.InitAsync();
}
}

class Program
{
static async Task Main(string[] args)
{
    var foo = await Foo.CreateAsync();
}
}
````


#### Factory (aka a factory class)
- It can be argued the construction of the object is a seperate responsibility of what the object does 
- If you do require a factory, why not take that factory into another class?
- Example below, the only issie is that people can still make a Point through the constructor: 
````c#
public static class PointFactory
{
    public static Point3 NewCartesianPoint(double x, double y)
    {
        return new Point3(x, y);
    }

    public static Point3 NewPolarPoint(double rho, double theta)
    {
        return new Point3(rho * Math.Cos(theta), rho * Math.Sin(theta));
    }
}

public class Point3
{
    private double x, y;

    public Point3(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        var point = PointFactory.NewPolarPoint(1, Math.PI / 2);
        Console.WriteLine(point);
    }
}
````


#### Inner Factory
- Problem of the public constructor... one way is to make it internal if you have multiple assemblies 
- If you want it private in your own assembly:
  - PointFactory would need to be an inner class of Point... aka the "Inner Factory"
````c#
public class Point4
{
    private double x, y;

    public Point4(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
    }
    
    public static class Factory
    {
        public static Point4 NewCartesianPoint(double x, double y)
        {
            return new Point4(x, y);
        }

        public static Point4 NewPolarPoint(double rho, double theta)
        {
            return new Point4(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var point = Point4.Factory.NewPolarPoint(1, Math.PI / 2);
        Console.WriteLine(point);
    }
}
````


#### Abstract Factory
- Why an abstract factory?
  - To give out abstact objects as opposed to concrete objects 
- In an abstract setting, you are not returning the types you are creating. You're returning abstract classes / interfaces
- TBH didn't follow this super well: 
````c#
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
    public enum AvailableDrink
    {
        Coffee, Tea
    }
    
    private Dictionary<AvailableDrink, IHotDrinkFactory> factories = new Dictionary<AvailableDrink,IHotDrinkFactory>();

    public HotDrinkMachine()
    {
        foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
        {
            var factory = (IHotDrinkFactory) Activator.CreateInstance(
                Type.GetType("DesignPatterns1.CreationalPatterns.Factories." +
                              Enum.GetName(typeof(AvailableDrink), drink) + "Factory")
            );
            factories.Add(drink, factory);
        }
    }
    
    public IHotDrink MakeDrink(AvailableDrink drink, int amount)
    {
        return factories[drink].Prepare(amount);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var machine = new HotDrinkMachine();
        var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Tea, 100);
        drink.Consume();
    }
}
````


### Summary on Factories
- A factory method is a static method that creates objects
- A factory can take care of object creation
- A factory can be external or reside inside the object as an inner class
- Hierarchies of factories can be used to create related objects


---


# Prototype
- Contents: 
  - ICloneable is bad
  - Copy Constructors
  - Explicit Deep Copy Interface
  - Copy through Serialization
  - Summary


- When its easier to copy an existing object than fully initialize a new one
- All about object copying / replicating 
- Complicated objects (e.g. cars) aren't designed from scratch. They take an existing design and improve it somehow, they reiterate existing designs. 
  - Prototype design pattern is the same thing, reiterating existing designs
- We make a copy (clone) of the prototype and customize it 
  - Requires "deep copy" support, need all the objects references etc. Changing this object only affects the new cloned object, not the object that was just copied.
- We make the cloning convenient (e.g. via a factory)


- Prototype: A partially or fully initizlied object that you "deep copy" (clone) and make use of.


---


### ICloneable is bad
- Life without a deep copy (doesn't work):
````c#
public class Person
{
    public string[] Names;
    public Address Address;

    public Person(string[] names, Address address)
    {
        this.Names = names;
        this.Address = address;
    }

    public override string ToString()
    {
        return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
    }
}

public class Address
{
    public string StreetName;
    public int HouseNumber;

    public Address(string streetName, int houseNumber)
    {
        StreetName = streetName;
        HouseNumber = houseNumber;
    }


    public override string ToString()
    {
        return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        var john = new Person(
            new []{"John", "Smith"},
            new Address("London Road", 123)
            );
        var jane = john; //this is not a copy, just points to the same reference! 
        jane.Names[0] = "Jane"; //causes both to say Jane, because you've just modified the reference. Not a "deep copy" 
        
        Console.WriteLine(john);
    }
}
````
- can instead implement "ICloneable" 
````c#
public class Person : ICloneable
{
    public string[] Names;
    public Address Address;

    public Person(string[] names, Address address)
    {
        this.Names = names;
        this.Address = address;
    }

    public override string ToString()
    {
        return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }
}
````
- the problem with cloning is that we're not sure if you're copying deeply the insides - i.e. are you deep copying the Address object too, or are you just getting a reference of the address? 
- You'd have to implement Clone() on the Address object too, and again cast it to the correct object etc. 
- Not the way to go because its badly specified (returns object), and we don't know what to do with it... 


---


### Copy Constructors
- Another way to copy objects: using copy constructors
- "Copy constructor" is a C++ term, lets you specify an object to copy the data from. 
````c#
public class Person : ICloneable
{
    public string[] Names;
    public Address Address;

    public Person(string[] names, Address address)
    {
        this.Names = names;
        this.Address = address;
    }

    public override string ToString()
    {
        return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
    }

    //copy constructor: 
    public Person(Person other)
    {
        Names = new [] {other.Names[0], other.Names[1]};
        Address = new Address(other.Address);
    }
}

public class Address
{
    public string StreetName;
    public int HouseNumber;

    public Address(string streetName, int houseNumber)
    {
        StreetName = streetName;
        HouseNumber = houseNumber;
    }
    
    public override string ToString()
    {
        return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
    }

    public Address(Address other)
    {
        StreetName = other.StreetName;
        HouseNumber = other.HouseNumber;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var john = new Person(
            new []{"John", "Smith"},
            new Address("London Road", 123)
            );

        //copy Constructors:
        var jane = new Person(john);
        jane.Names[0] = "Jane";
        jane.Address.HouseNumber = 321;
        
        Console.WriteLine(john);
        Console.WriteLine(jane);
    }
}
````


---


### Explicit Deep Copy Interface
- ICloneable doesn't work for us, but maybe our own interface design will. 
  - If you explicitly specify that the interface does a deep copy then theres no problem
````c#
public interface IPrototype<T>
{
    T DeepCopy();
}

public class Person : IPrototype<Person>
{
    public string[] Names;
    public Address Address;

    public Person(string[] names, Address address)
    {
        this.Names = names;
        this.Address = address;
    }

    public override string ToString()
    {
        return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}: {Address}";
    }

    //deep copy interface:
    public Person DeepCopy()
    {
        return new Person(Names, Address.DeepCopy());
    }
}

public class Address : IPrototype<Address>
{
    public string StreetName;
    public int HouseNumber;

    public Address(string streetName, int houseNumber)
    {
        StreetName = streetName;
        HouseNumber = houseNumber;
    }
    
    public Address DeepCopy()
    {
        return new Address(StreetName, HouseNumber);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var john = new Person(
            new []{"John", "Smith"},
            new Address("London Road", 123)
            );

        // Explicit deep copy interface
        var jane = john.DeepCopy();
        jane.Address.HouseNumber = 321;
        
        Console.WriteLine(john);
        Console.WriteLine(jane);
    }
}
````


---


### Copy Through Serialization
- Adding copy logic to all the different types is really tedius and we what to get away from that
- We can get all this functionality automatically using a serializer: serializing it and deserializing it gives a deep copy 
- Serialization is how the Prototype Pattern is used in the real world
- Every type that you try to serialize using the binary formatter has to be serializable (as well as every member) 
  - The cavet here is that you need to go through every single class and add the [Serialize] tag, but we can get away from that...
  - No one is forcing us to use binary serialization - we can use another formatter like Xml Serialization
  - One of the cavets of XmlSerializers is that you have to have paramaterless constructors on each of your classes
````c#
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
}

class Program
{
    static void Main(string[] args)
    {
        var john = new Person(
            new []{"John", "Smith"},
            new Address("London Road", 123)
            );

        //Serialization: 
        var jane = john.DeepCopyXml();
        jane.Names[0] = "Jane";
        jane.Address.HouseNumber = 321;
        
        Console.WriteLine(john);
        Console.WriteLine(jane);
    }
}
````


---


### Summary
- To implement a prototype, partially construct an object and store it somewhere
- Clone the prototype 
  - impelement your own deep copy functionality (making copy constructors all over the place or inheriting a copy interface and implement it everywhere)
  - simpler way is to serialize and deserialize
  - customize the resulting instance and start using it in your code 


---


# Singleton 
- A design pattern everyone loves to hate ... is it really that bad?
- the raw use of singleton is often a design smell 
- Motivating for using singleton:
  - For some components it only makes sense to have one in the system
    - I.e. a database repository 
    - Object factory 
  - E.g. the constructor call is expensive, and you only want this constructor call being done once
    - we only want to do it once
    - we provide everyone with the same instance
  - We want to prevent anyone creating additional copies
  - We need to take care of lazy instantation and thread safety


- Singleton: A component which is instantiated only once, and tries to resist being instantiated more than once. 


----


### Singleton Implementation
- One thing you can do to stop others from using it, is to simply make the class constructor private. 
````c#
public interface IDatabase
{
    int GetPopulation(string name);
}

public class SingletonDatabase : IDatabase
{
    private Dictionary<string, int> capitals;

    private SingletonDatabase()
    {
        Console.WriteLine("Initializing Database");

        capitals = File.ReadAllLines("Capitals.txt")
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
    
    private static SingletonDatabase instance = new SingletonDatabase();
    // to actually get the instance, you have to call the static instance method.
    // There is no way to have more than one instance, it will only refer to the one reference above.
    public static SingletonDatabase Instance => instance;  
}
````
- The above constructor will cause you to read a file / database - something you want to avoid if the client doesn't actually need it 
- Can change it to use the lazy implementation
- This construct allows you to only create the singleton db when somebody accesses the instance. like so: 
````c#
//...       ;
    private static Lazy<SingletonDatabase> instance = 
        new Lazy<SingletonDatabase>(() => new SingletonDatabase());

    public static SingletonDatabase Instance => instance.Value;
}  
````


---


### Testability Issues
- We really want a fake object for the database, but we cannot get it 
- The reason is the SingletonRecordFinder has a hardcoded reference to the instance (this is the problem with the singleton)
- Singleton patterns require hardcoding, cannot substitute with something else.
````c#
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

[TestFixture]
public class SingletonTests
{
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
}
````


---


### Singleton in Dependency Injection
- using dependency injection: 
````c#
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
    public void ConfigurablePopulationTest()
    {
        var recordFinder = new ConfigurableRecordFinder(new MockDatabase());
        var names = new[] {"alpha", "beta"};
        int tp = recordFinder.GetTotalPopulation(names);

        Assert.That(tp, Is.EqualTo(3));
    }
}
````
- in the real world instead building a singleton yourself, you delegate the responsibility to the dependency injection container. 
````c#
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
````
- Implemented with the dependency injection framework something like this:
  - this means there is a single point where you can change one to the other when you want to test things for example 
````c#
[Test]
public void DIPopulationTest()
{
    var cb = new ContainerBuilder(); //using Autofac nuget package
    cb.RegisterType<OrdinaryDatabase>()
        .As<IDatabase>()
        .SingleInstance(); //this is how you tell the DI Container that its a singleton
    cb.RegisterType<ConfigurableRecordFinder>();
    using (var c = cb.Build())
    {
        var rf = c.Resolve<ConfigurableRecordFinder>();
    }
}
````


---


### Monostate Pattern
- If you want a singleton, why not just make the whole thing static? 
  - A static class with static members doesn't have a constructor (cannot use things like DI - testability is bad)
- A variation on singleton pattern is "monostate" which tries to use static class
- Monostate pattern has the state being static, but being exposed in a non-static way
- Allows for multiple instatiations, but they all refer to the same object...
- Bizarre and confusing pattern, since its pretty counter intuitive whats going on
````c#
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

class Program
{
    static void Main(string[] args)
    {
        var ceo = new CEO();
        ceo.Name = "Adam Smith";
        ceo.Age = 55;
        
        var ceo2 = new CEO();
        
        Console.WriteLine(ceo2); //because the name and age are static, they refer to the same object
    }
}
````


---


### Per-Thread Singleton
- Lazy gives us thread safety during initialization
- Instead can have one singleton per thread 
- Really up to you to find scenarios where this is relevant
````c#
public sealed class PerThreadSingleton
{
    private static ThreadLocal<PerThreadSingleton> threadInstance 
        = new ThreadLocal<PerThreadSingleton>(
            () => new PerThreadSingleton());

    public int Id;
    
    private PerThreadSingleton()
    {
        Id = Thread.CurrentThread.ManagedThreadId;
    }

    public static PerThreadSingleton Instance => threadInstance.Value; //how u expose it
}

class Program
{
    static void Main(string[] args)
    {
        var t1 = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"t1: {PerThreadSingleton.Instance.Id}");
        });
        
        var t2 = Task.Factory.StartNew(() =>
        {
            Console.WriteLine($"t2: {PerThreadSingleton.Instance.Id}");
            Console.WriteLine($"t2: {PerThreadSingleton.Instance.Id}");
        });

        Task.WaitAll(t1, t2);
    }
}
````


---


### Ambient Context Pattern
- ambient means "present everywhere" 
- here we specifiy the height in every wall 
````c#
static void Main(string[] args)
{
    var house = new Building();
    //ground, height is 3000
    house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0), 3000)); //last argument is the height 
    house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000), 3000));
    
    //1st floor, height is 3500
    house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0), 3500));
    house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000), 3500));

    //ground, height is 3000
    house.Walls.Add(new Wall(new Point(5000, 0), new Point(5000, 4000), 3000));
}
````
- the height below is an ambient context;
````c#
static void Main(string[] args)
{
    var house = new Building();
    //ground, height is 3000
    var height = 3000;
    house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0), height));
    house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000), height));
    
    //1st floor, height is 3500
    height = 3500;
    house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0), height));
    house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000), height));

    //ground, height is 3000
    height = 3000;
    house.Walls.Add(new Wall(new Point(5000, 0), new Point(5000, 4000), height));
}
````
- every wall has a height, and for any group of walls that height would be the same value, so its ambient, aka present everywhere
- so why do we have an argument? why don't we just have a specified location where these values are kept? this is what is meant by ambient context
````c#
public class BuildingContext //typically this would be a singleton, or you wouldn't construct any instances of it
{
    public static int WallHeight;
}

public class Building
{
    public List<Wall> Walls = new List<Wall>();
}

public struct Point
{
    private int x, y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Wall
{
    public Point Start, End;
    public int Height;

    public Wall(Point start, Point end)
    {
        Start = start;
        End = end;
        Height = BuildingContext.WallHeight; //now always refers to this! no need for an argument
    }
}

class Program
{
    static void Main(string[] args)
    {
        var house = new Building();
        //ground, height is 3000
        BuildingContext.WallHeight = 3000;
        house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0))); // wall height taken out 
        house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000)));
        
        //1st floor, height is 3500
        BuildingContext.WallHeight = 3500;
        house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0)));
        house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000)));

        //ground, height is 3000
        BuildingContext.WallHeight = 3000;
        house.Walls.Add(new Wall(new Point(5000, 0), new Point(5000, 4000)));
    }
}
````
- this approach is not thread-safe, but there are solutions for that ...


### Summary
- making a "Safe" singleton is easy, construct a static Lazy<T> and return its Value
- Singletons are difficult to test
- Instead of directly using a singleton, consider dependning on an abstraction (i.e. an interface)
- Consider defining singleton lifetime in DI container. This is the socially acceptable way of using a singleton