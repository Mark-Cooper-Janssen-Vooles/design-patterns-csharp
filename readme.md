# Design Patterns in C# and .NET


- Introduction
- The SOLID design principles
- Creational Patterns (dealing with creation of objects):
  - Builder
  - Factories
    - Abstract Factory
    - Factory Method
  - Prototype
  - Singleton
- Structural Patterns (dealing with structure of classes): 
  - Adapter
  - Bridge
  - Composite
  - Decorator 
  - Facade
  - Flyweight
  - Proxy
- Behavioral Patterns (all different, no central theme):
  - Chain of Responsibility
  - Command
  - Interpreter
  - Iterator
  - Mediator 
  - Memento
  - Null Object
  - Observer
  - State
  - Strategy
  - Template Method 
  - Visitor 
- Summary


### Gamma Categorization
- Creational patterns
  - Deal with the creation (construction) of objects
  - Explicit (construction) vs implicit (DI, reflection, etc)
  - Wholesale (single statement - i.e. 1 constructior fully initialises the object) vs piecewise (step-by-step)
- Structural Patterms
  - Concerned with the structure (e.g. class members)
  - Many patterns are wrappers that mimic the underlying class interface
  - Stress the importance of good API design
- Behavioral Patterns
  - They are all different; no central theme 


---


## Introduction 
- Design patterns are common architectural approaches observed in engineering practices 
- Design patterns are object oriented patterns
- A pattern is a description of the approach, how it is expressed in the programming language, and what its actually used for 
- Popularised by the Gang of Four book (1994)
- Course goes through SOLID design principles, then Creational patterns / Structural Patterns / Behavioral patterns


---


## SOLID Design Principles
- Solid is frequently referenced in design pattern literature 


Single Responsibility Principle:
- Any particular class should just have a single reason to change
- Don't add too much responsibility to a single class
````c#
// too much responsibility for one class:
public class Journal
{
    private readonly List<string> entries = new List<string>();
    private static int _count = 0;

    public int AddEntry(string text)
    {
        entries.Add($"{++_count}: {text}");
        return _count; //memento pattern
    }

    public void RemoveEntry(int index)
    {
        entries.RemoveAt(index);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, entries);
    }

    public void Save(string filename)
    {
        File.WriteAllText(filename, ToString());
    }

    public static Journal Load(string filename)
    {
    }

    public void Load(uri uri)
    {
        
    }
}
````
- Instead do something like this:
````c#
public class Journal
{
    private readonly List<string> entries = new List<string>();
    private static int _count = 0;

    public int AddEntry(string text)
    {
        entries.Add($"{++_count}: {text}");
        return _count; //memento pattern
    }

    public void RemoveEntry(int index)
    {
        entries.RemoveAt(index);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, entries);
    }
}

public class Persistence
{
    public void SaveToFile(Journal j, string filename, bool overwrite = false)
    {
        if (overwrite || !File.Exists(filename))
            File.WriteAllText(filename, j.ToString());
    }
}

class Program
{
    static void Main(string[] args)
    {
        var j = new Journal();
        j.AddEntry("I cried today");
        j.AddEntry("I ate a bug");

        Console.WriteLine(j);
        
        var p = new Persistence();
        var filename = @"c:\temp\journal.txt";
        p.SaveToFile(j, filename, true);
    }
}    
````


---


## Open-Closed Principle
- A class should be open for extension, but closed for modification. 
- People should be able to extend the functionality of a class, but not by modifying that class directly 
````c#
//How not to do it, each "productFilter" method was added incrementally after the boss wanted a new functionality: 
public class ProductFilter
{
    public IEnumerable<Product> FilterBySize(IEnumerable<Product> product, Size size)
    {
        foreach (var p in product)
        {
            if (p.Size == size)
                yield return p;
        }
    }
    
    public IEnumerable<Product> FilterByColor(IEnumerable<Product> product, Color color)
    {
        foreach (var p in product)
        {
            if (p.Color == color)
                yield return p;
        }
    }
    
    public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> product, Color color, Size size)
    {
        foreach (var p in product)
        {
            if (p.Color == color && p.Size == size)
                yield return p;
        }
    }
}
````
- How to do it using the Open-Closed principle:
````c#
//an enterprise pattern called "the specification pattern"

public interface ISpecification<T> //works on anything
{
    bool IsSatisfied(T t); //
}

public interface IFilter<T>
{
    IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
}

public class ColorSpecification : ISpecification<Product>
{
    public Color color;

    public ColorSpecification(Color color)
    {
        this.color = color;
    }
    
    public bool IsSatisfied(Product t)
    {
        return t.Color == color;
    }
}

public class SizeSpecification : ISpecification<Product>
{
    public Size Size;

    public SizeSpecification(Size size)
    {
        this.Size = size;
    }
    
    public bool IsSatisfied(Product t)
    {
        return t.Size == Size;
    }
}

public class BetterFilter : IFilter<Product>
{
    public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
    {
        foreach (var i in items)
            if (spec.IsSatisfied(i))
                yield return i;
    }
}

public class AndSpecification<T> : ISpecification<T>
{
    private ISpecification<T> _first, _second;

    public AndSpecification(ISpecification<T> first, ISpecification<T> second)
    {
        this._first = first;
        this._second = second;
    }
    public bool IsSatisfied(T t)
    {
        return _first.IsSatisfied(t) && _second.IsSatisfied(t);
    }
}
````
- parts of the system have to be open for extension, but closed for modification. I.e. generate new classes via inheritance of interfaces, rather than modifiying existing classes



---


## Liskov Substitution Principle
- You should be able to substitue a base type for a sub type
````c#
//violating this principle: 
public class Rectangle
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Rectangle()
    {
        
    }

    public Rectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
    }
}

public class Square : Rectangle
{
    public new int Width
    {
        set
        { base.Width = base.Height = value; }
    }

    public new int Height
    {
        set { base.Width = base.Height = value; }
    }
}

//in main:
Rectangle rc = new Rectangle(2, 3);
Console.WriteLine($"{rc} has area {Area(rc)}");

Square sq = new Square(); 
//according to the liskov substitution princple, you should be able to change this to a rectangle and nothing should go wrong.
//However we'd now have a width of 4 and a height of 0 (default int)
//You should be able to "upcast" to the base type, and be okay. But the way this is done here violates that 
sq.Width = 4;
Console.WriteLine($"{sq} has area {Area(sq)}");
````
- An example of how to fix this:
````c#
 public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public Rectangle()
    {
        
    }

    public Rectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
    }
}

public class Square : Rectangle
{
    public override int Width
    {
        set
        { base.Width = base.Height = value; }
    }

    public override int Height
    {
        set { base.Width = base.Height = value; }
    }
}

//note: best not to use inheritance like this anyway imo... confusing 
````


---


### Interface Segregation Principle: 
- Interfaces should be segregated so that nobody implementing your interfaces has to implement functions or properties they don't actually need 
- Often caused by building interfaces that are too large
````c#
//i.e. you have a class "Printer" but different types etc, so you decide to build one big interface for everything. "IMachine"
// violating this principle: 
public interface IMachine
{
    void Print(Document d);
    void Scan(Document d);
    void Fax(Document d);
}

public class MultifunctionPrinter : IMachine
{
    public void Print(Document d)
    {
        //
    }

    public void Scan(Document d)
    {
        //
    }

    public void Fax(Document d)
    {
        //
    }
}

public class OldPrinter : IMachine
{
    public void Print(Document d)
    {
        //
    }

    public void Scan(Document d)
    {
        throw new System.NotImplementedException(); //the old fashion printer can't scan - but you have to implement this. do you throw an error? what do you do?
    }

    public void Fax(Document d)
    {
        throw new System.NotImplementedException(); //can't fax either
    }
}
````
- If you have an interface that includes too many things, simply break it apart into smaller interfaces:
````c#
public interface IPrinter
{
    void Print(Document d);
}

public interface IScanner
{
    void Scan(Document d);
}

public interface IFax
{
    void Fax(Document d);
}


public class OldPrinter : IPrinter
{
    public void Print(Document d)
    {
        //
    }
}

//can do this:
public class Photocopier : IPrinter, IScanner
{
    public void Print(Document d)
    {
        //
    }

    public void Scan(Document d)
    {
        //
    }
}

//or this:
public interface IMultiFunctionDevice : IScanner, IPrinter, IFax
{
    
}

public class MultifunctionPrinter : IMultiFunctionDevice
{
    public void Scan(Document d)
    {
        //
    }

    public void Print(Document d)
    {
        //
    }

    public void Fax(Document d)
    {
        //
    }
}
````


---


### Dependency Inversion Principle
- High level parts of the system should not depend on low-level parts of the system directly, they should depend on abstraction
````c#
//violating this principle: 
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

//low-level
public class Relationships
{
    private List<(Person, Relationship, Person)> relations = new List<(Person, Relationship, Person)>();

    public void AddParentAndChild(Person parent, Person child)
    {
        relations.Add((parent, Relationship.Parent, child));
        relations.Add((child, Relationship.Child, parent));
    }

    public List<(Person, Relationship, Person)> Relations => relations;
}

public class Research
{
    public Research(Relationships relationships)
    {
        var relations = relationships.Relations;
        foreach (var r in relationships.Relations.Where(x =>
            x.Item1.Name == "John" && x.Item2 == Relationship.Parent))
        {
            Console.WriteLine($"John has a child called {r.Item3.Name}");
        }
        
        //the issue with this code is that we're accessing a very low-level data store and exposing a private thing as public
        // in practice this means Relationships cannot change its mind on how to store the relations 
    }
}
````
- A better way that allows Relationships to change is to provide a form of abstraction by defining an interface for the Relationships class.
````c#
public enum Relationship
{
    Parent, 
    Child,
    Sibling
}

public class Person
{
    public string Name;
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
````
- Now Relationships can change the way it stores relations, because its never exposed to the high-level modules that are actually consuming it.



---


### Summary of SOLID
- Single Responsibility Principle
  - A class should only have one reason to change
  - Separation of concerns - different classes handling different, independent tasks/problems
- Open-closed Principle
  - classes should be open for extension but closed for modification
- Liskov substititon principle 
  - you should be able to substitute a base type for a subtype 
- Interface segregation Principle
  - don't put too much into an interface; split into separate interfaces
  - YAGNI - you ain't going to need it 
- Dependency Inversion Principle
  - high-level modules should not depend upon low-level ones; use abstractions


### Summary of Creational Patterns:
- Builder: Provides an API for constructing an object step-by-step (piecewise).
    - Separate component for when object construction gets too complicated (too many args, too much logic in constructor, etc)
    - Can create mutually cooperating sub-builders 
    - Often has a fluent interface (return this at end of invokation)
- Factory: A component responsible solely for the wholesale (not piecewise) creation of objects 
    - Factory method more expressive than a constructor, different name, can have overloads
    - Factory can be an outside class or inner class; inner class has the benefit of accessing private members
- Prototype: A partially or fully initizlied object that you "deep copy" (clone) and make use of.
    - Creation of object from an existing object
    - Requires either explicit deep copy or copy through serialization
- Singleton: A component which is instantiated only once, and tries to resist being instantiated more than once
    - When you need to ensure just a single instance exists 
    - Made thread-safe and lazy with Lazy<T>
    - Consider extracting interface or using dependency injection (the only socially acceptable way of using a singleton today is via DI)


### Summary of Structural Patterns: 
- Adapter: A construct which adapts an existing interface X to conform to the required interface
    - Converts the interface you get to the interface you need. In adapter, it can spawn additional objects. It might make sense to cache the results of the adaptation for subsequent calls
- Bridge: A mechanism that decouples an interface (hierarchy) from an implementation (hierarchy).
    - Decouple abstraction from implementation
- Composite: A mechanism for treating individual (scalar) objects and compositions of objects in a uniform manner
    - Allows clients to tread individual objects and compositions of objects uniformly
- Decorator: Facilitates the addition of behaviors to individual objects without inheriting from them 
    - Attach additional responsibilities to objects 
    - 'Inherit' from sealed classes; emulate multiple inheritance
- Facade: Provides a simple, east to understand / user interface over a large and sophisticated body of code 
    - Provide a single unified interface over a set of classes / system 
- Flyweight: A space optimization technique that lets us use less memory by storing externally the data associated with similar objects.
    - Efficiently support very large numbers of similar objects
- Proxy: Providing a class that functions as an interface to a particular resource. That resource may be remote, expensive to construct, or may require logging or some other added functionality.
    - Provide a surrogate object that forwards calls to the real object while performing additional functions
    - Dynamic proxy creates a proxy dynamically, without the necessity of replicating the target object API (frequently done in .NET)


### Summary of Behavioural Patterns:
- Chain of responsibility: A chain of components who all get a chance to process a command or a query, optionally having default processing implementation and an ability to terminate the processing chain
    - Allows components to process information / events in a chain
    - Each element in the chain refers to next element or;
    - Make a list and go through it
- Command: Lets you build an object which represents an instruction to perform a particular action. This command contains all the information necessary for the action to be taken. 
    - Encapsulate a request into a separate object
    - Good for audit, replay, undo/redo
    - Part of CQS (command query separation)
- Interpreter: A component that processes structured text data. Does so by turning it into separate lexical tokens (lexing) and then interpreting seqences of said tokens (parsing)
    - Transform textual input into object-oriented structures
    - Used by interpreters, compilers, static analysis tools, etc
    - Compiler theory is a separate branch of computer science 
- Iterator: An object (or, in .NET, a method using IEnumerable<T>) that facilitates the traversal of a data structure
    - Provides an interface for accessing elements of an aggregate object
    - IEnumerable<T> should be used in 99% of cases
- Mediator: A component that facilitates communication between other components without them necessarily being aware of each other or havng direct (reference) access to each other. 
    - Provides mediation services between two objects
    - E.g. message passing, chat room
- Memento: A token / handle representing the system state. Lets us roll back to the state when the token was generated. May or may not directly expose state information. 
    - Yields tokens representing system states
    - Tokens do not allow direct manipulation, but can be used in appropriate APIs
- Observer: An observer is an object that wishes to be informed about events happening in the system. The entity generating the events is an observable. 
    - Built into C# with the event keyword 
    - Additional support provided for properties, collections and observable stream 
- State: 
    - We model systems by having one of a possible states and transitions between these states
    - Such a system is called a state machine
    - Special frameworks exist to orchestrate state machines
- Stratergy & Template methods:
    - Strategy pattern: Enables the exact behavior of a system to be selected either at run-time (dynamic) or compile-time (static).
        - also known as a policy (esp in the C++ world)
    - Template method: Allows us to define the 'skeleton' of the algorithm, with concrete implementations defined in subclasses.
    - Both patterns define an algorithm blueprint / placeholder
    - Stratergy uses composition, template method uses inheritance 
- Visitor pattern: A pattern where a component (visitor) is allowed to traverse the entire inheritance hierarchy. Implemented by propagating a single visit() (typically called visit()) method throughout the entire hierarchy. It allows 'double dispatch'.
    - Adding functionality to existing classes through double dispatch