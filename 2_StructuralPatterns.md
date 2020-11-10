(Note: Got up to lecture 44 then stopped watching fully, just took down overview of patterns. Didn't wanna get bogged down in details as I might never see the actual pattern and it might not help anyway)


Dealing with the structure of classes


Contents:
- Adapter
- Bridge
- Composite
- Decorator 
- Facade
- Flyweight
- Proxy


# Adapter
- Getting the interface you want from the interface you have 
- think of a powerpoint adapter, they have different interface requirements
  - voltage
  - socket/plug type
- We cannot modify our gadgets to support every possible interface
- Thus we use a special device (an adapter) to give us the interface we require from the interface we have


- Adapter: A construct which adapts an existing interface X to conform to the required interface Y


### Vector/Raster Demo
- Adapter pattern all about conforming an interface you have to an interface you're given 
````c#
public class Point
{
    public int X, Y;

    public Point(int x, int y)
    {
        X = X;
        Y = Y;
    }
}

public class Line
{
    public Point Start, End;

    public Line(Point start, Point end)
    {
        Start = start;
        End = end;
    }
}

public class VectorObject : Collection<Line>
{
    
}

public class VectorRectangle : VectorObject
{
    public VectorRectangle(int x, int y, int width, int height)
    {
        Add(new Line(new Point(x, y), new Point(x + width, y)));
        Add(new Line(new Point(x + width, y), new Point(x+ width, y + height)));
        Add(new Line(new Point(x, y), new Point(x, y + height)));
        Add(new Line(new Point(x, y+height), new Point(x + width, y + height)));
    }
}

public class LineToPointAdapter : Collection<Point>
{
    private static int count;

    public LineToPointAdapter(Line line)
    {
        Console.WriteLine($"{++count}: Generating points for line [{line.Start.X}, {line.Start.Y}]-[{line.End.X}-{line.End.Y}]");
        int left = Math.Min(line.Start.X, line.End.X);
        int right = Math.Max(line.Start.X, line.End.X);
        int top = Math.Min(line.Start.Y, line.End.Y);
        int bottom = Math.Max(line.Start.Y, line.End.Y);
        int dx = right - left;
        int dy = line.End.Y - line.Start.Y;

        if (dx == 0)
        {
            for (int y = top; y <= bottom; ++y)
            {
                Add(new Point(left, y));
            }
        } else if (dy == 0)
        {
            for (int x = left; x <= right; ++x)
            {
                Add(new Point(x, top));
            }
        }
    }
}

class Program
{
    private static readonly List<VectorObject> vectorObjects = new List<VectorObject>
    {
        new VectorRectangle(1, 1, 10, 10),
        new VectorRectangle(3, 3, 6, 6)
    };
    public static void DrawPoint(Point p)
    {
        Console.WriteLine(".");
    }
    
    static void Main(string[] args)
    {
        Draw();
        Draw();
    }

    private static void Draw()
    {
        foreach (var vo in vectorObjects)
        {
            foreach (var line in vo)
            {
                var adapter = new LineToPointAdapter(line);
                adapter.ForEach(DrawPoint);
            }
        }
    }
}
````


### Adapter Caching 
- A downside of adapater pattern is that you generate a lot of temporary information
- Caching: "simply preserve the information that you generated for future use"
````c#
// can do something like this, check lecture 43 for details:
public class LineToPointAdapter : Collection<Point>
{
    private static int count;
    static Dictionary<int, List<Point>> cache = new Dictionary<int, List<Point>>();
````


### Generic Value Adapter
- Another adapter variation
- He thinks is really silly, unfortunately necessary in c# 
- Suppose you want to implement vectors, i.e. Vector2f, Vector3i
````c#
//i.e. we want to make different vectors, like Vector2f, Vector3i
public class Vector<T, D>
{
  protected T[] data;

  public Vector()
  {
    data = new T[D]; //in c# we cannot put D here, so we're unsure if its a float or an integer
  }
}
````
- Solved by adapting a literal value to a type 
````c#
public interface IInteger
{
  int Value { get; }
}

public class Two : IInteger
{
  public int Value => 2;
}

//thus can call it like
public class Vector<T, D>
  where D: IInteger, new ()
{
  protected T[] data;

  public Vector()
  {
    data = new T[new D().Value];
  }
}
````


### Adapater in Dependency Injection
- how a dependency injection framework helps you build and inject adapters


### Exercise: 
````c#
public class Square
{
    public int Side;
}

public interface IRectangle
{
    int Width { get; }
    int Height { get; }
}

public static class ExtensionMethods
{
    public static int Area(this IRectangle rc)
    {
        return rc.Width * rc.Height;
    }
}

public class SquareToRectangleAdapter : IRectangle
{
    public SquareToRectangleAdapter(Square square)
    {
        Width = square.Side;
        Height = square.Side;
    }

    public int Width { get; }
    public int Height { get; }
}
````


### Summary Adapter
- Implementing an adapter is easy
- Determine the API you have and the API you need to provide
- Create a component which aggregates (has reference to..) the adaptee
- Intermediate representations can pile up: use caching and other optimizations


---


# Bridge
- Connecting compoments together through abstractions (interfaces or abstract classes)
- Motivation
  - Avoids "cartesian product" complexity explosion
    -e.g. Base class ThreadSchedule
    - Can be preemptive or cooperative
    - Can be run on Windows or Unix
    - End up with a 2x2 scenario (4 different products: WindowsPTS, UnixPTS, WindowsCTS, UnixCTS)
    - I.e. ThreadScheduler inherits from PremptiveThreadScheduler OR Cooperative threadScheduler
    - PreemptiveThreadSchedule inherits from WindowsPTS OR UnixPTS etc, the tree can get massive
  - Bridge pattern avoids the entity explosion
  - I.e. ThreadScheduler inherits from PreemptiveThreadScheduler OR CooperativeThreadScheduler. Then give threadScheduler a private field called PlatformScheduler (which is injected as IPlatformScheduler, which is UnixScheduler or WindowsScheduler)


- Bridge: A mechanism that decouples an interface (hierarchy) from an implementation (hierarchy).


### Bridge Example
- You have an IRenderer that renders for multiple renderers
- You have VectorRenderer and IRenderer that inherit from IRenderer
- The abstract Shape class takes a renderer in its constructor
  - Note: an abstract class is like an interface, except the abstract class can provide implementation (i.e. force inheriting classes to use a certain implementation) whereas interfaces cannot. 
- the class Circle inherits from Shape, this in Circle itself you now have access to the specific renderer. Instead of needing "CircleRenderer" or "VectorRenderer" classes, you've cut down your classes
````c#
public interface IRenderer
{
    void RenderCircle(float radius);
}

public class VectorRenderer : IRenderer
{
    public void RenderCircle(float radius)
    {
        Console.WriteLine($"Drawing a cirlce of {radius}");
    }
}

public class RasterRenderer : IRenderer
{
    public void RenderCircle(float radius)
    {
        Console.WriteLine($"Drawing pixels fr circle with radius {radius}");
    }
}

public abstract class Shape
{
    protected IRenderer renderer;

    protected Shape(IRenderer renderer)
    {
        this.renderer = renderer;
    }

    public abstract void Draw();
    public abstract void Resize(float factor);
}

public class Circle : Shape
{
    private float radius;

    public Circle(IRenderer renderer, float radius) : base(renderer)
    {
        this.radius = radius;
    }

    public override void Draw()
    {
        renderer.RenderCircle(radius);
    }

    public override void Resize(float factor)
    {
        radius *= factor;
    }
}

class Program
{
    static void Main(string[] args)
    {
        //var renderer = new RasterRenderer();
        var renderer = new VectorRenderer();
        var circle = new Circle(renderer, 5);
        
        circle.Draw();
        circle.Resize(2);
        circle.Draw();
    }
}
````
- doing this with dependency injection: 
````c#
static void Main(string[] args)
{
    var cb = new ContainerBuilder();
    cb.RegisterType<VectorRenderer>().As<IRenderer>(); //whenever anyone asks for an IRenderer, inject a vectorRenderer
    cb.Register((C, p) =>
        new Circle(C.Resolve<IRenderer>(),
            p.Positional<float>(0)));

    using (var c = cb.Build())
    {
        var circle = c.Resolve<Circle>(
            new PositionalParameter(0, 5.0f)
        );
        circle.Draw();
        circle.Resize(2.0f);
        circle.Draw();
    }
}
````


### Bridge Summary
- Decouple abstraction from implementation 
- Both can exist as hierarchies 
- A stronger form of encapsulation 


---


# Composite Pattern
- goal is to allow us to treat individual objects and aggregate objects in the same manner
- Treating individual and aggregate objects uniformly 
- Motivation:
  - Objects use other objects fields / properties / members through inheritance or composition (i.e. injecting it through the constructor)
    - i.e. a mathematical expression composed of simple expressions or
    - a grouping of shapes that consists of several shapes 
  - Compositve design pattern is used to treat both single (scalar) and compoisite (grouped objects - i.e. an object that is a collection of other objects) objects in the same way.
    - i.e. Foo and Collection<Foo> have common APIs, which you can then call on one or the other (without knowing if you're working on a single object or a collection)


- Composite: A mechanism for treating individual (scalar) objects and compositions of objects in a uniform manner


### Geometric Shapes
- You have an object potentially consisting of objects of its own type
- If you don't need a list you don't get it, because its wrapped in a lazy constructor 
````c#
public class GraphicObject
{
    public string Colour;
    public virtual string Name { get; set; } = "Group";
    
    private Lazy<List<GraphicObject>> children = new Lazy<List<GraphicObject>>();
    public List<GraphicObject> Children => children.Value;

    private void Print(StringBuilder sb, int depth)
    {
        sb.Append(new string('*', depth))
            .Append(string.IsNullOrWhiteSpace(Colour) ? string.Empty : $"{Colour} ")
            .AppendLine(Name);
        foreach (var child in Children)
        {
            child.Print(sb, depth + 1);
        }
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        Print(sb, 0);

        return sb.ToString();
    }
}

public class Circle : GraphicObject
{
    public override string Name => "Circle";
}

public class Square : GraphicObject
{
    public override string Name => "Square";
}

class Program
{
    static void Main(string[] args)
    {
        var drawing = new GraphicObject {Name = "My Drawing"};
        drawing.Children.Add(new Square {Colour = "Red"});
        drawing.Children.Add(new Circle {Colour = "Yellow"});
        
        var group = new GraphicObject();
        group.Children.Add(new Circle {Colour = "Blue"});
        group.Children.Add(new Square {Colour = "Blue"});
        
        drawing.Children.Add(group);

        Console.WriteLine(drawing);
    }
}

//prints: 
// My Drawing
// *Red Square
// *Yellow Circle
// *Group
// **Blue Circle
// **Blue Square
````


### Composite Summary:
- Objects can use other objects via inheritance / composition
- Some composed and singular objects need similar / identical behaviours
- Composite design pattern lets us treat both types of objects uniformly
- C# has special support for the enumeration concept
- A single object can masquerade as a collection with ``yield return this``
````c#
public interface IValueContainer : IEnumerable<int>
{
    
}

public class SingleValue : IValueContainer
{
    public int Value;
    
    public IEnumerator<int> GetEnumerator()
    {
        yield return Value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class ManyValues : List<int>, IValueContainer
{
    public List<int> Values;
    public int numberOfValues()
    {
        return Values.Count;
    }
}

public static class ExtensionMethods
{
    public static int Sum(this List<IValueContainer> containers)
    {
        int result = 0;
        foreach (var c in containers)
        foreach (var i in c)
            result += i;
        return result;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var manyValues = new ManyValues();
        manyValues.Add(1);
        manyValues.Add(7);
        
        
        var container = new List<IValueContainer> {new SingleValue() {Value = 1}, manyValues};

        var sum = ExtensionMethods.Sum(container);
        Console.WriteLine(sum);
    }
}
````


---


# Decorator
- Adding behavior without altering the class itself 
- Motivation:
  - Want to augment an object with additional functionality
  - Do not want to rewrite or alter existing code (open-closed principal)
  - Want to keep new functionality separate (single responsibility principle)
  - Need to be able to interact with existing structures
  - Two options:
    - Inherit from required object if possible + add some additional functionality; but some objects are sealed (thus cannot be inherited from) 
    - Instead you can build a decorator, which simply references the decorated object(s)


- Decorator: Facilitates the addition of behaviors to individual objects without inheriting from them 


### Custom String Builder 
- A decorator around a string builder class
  - Isn't the whole point to build a multiple inheritance thing? Not really, sometimes you just get sealed classes
- You cannot inherit from a sealed class (i.e. string builder)
- When putting "private StringBuilder builder = new StringBuilder();" field, alt + enter (in rider) and choose "Generate delegate members" => This is the crux of the Decorator pattern
- Now it does everything a string builder does! Except that it has a fluid interface, so its returning StringBuilder (and not our class, CodeBuilder)
- Need to change it to something like this:
````c#
//from this:
public StringBuilder Append(bool value)
{
    return builder.Append(value);
}
//to this:
public CodeBuilder Append(bool value)
{
    builder.Append(value);
    return this;
}
````
- Highlight all the code. Go Edit => Find => Find/Replace. Top search is "StringBuilder", bottom to replace is "CodeBuilder"
- Do the same for the internal of the functions, i.e. Top is "return Builder.(.+)$", bottom is "builder.$1\nreturn this" (make sure regex is on)
- End code looks something like this: 
````c#
namespace DesignPatterns1.StructuralPatterns.Decorator
{
    class Program
    {
        //public class CodeBuilder : StringBuilder //"cannot inherit from sealed class StringBuilder"
        //{}

        public class CodeBuilder
        {
            private StringBuilder builder = new StringBuilder();
            
            public override string ToString()
            {
                return builder.ToString();
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                ((ISerializable) builder).GetObjectData(info, context);
            }

            public CodeBuilder Append(bool value)
            {
                builder.Append(value);
                return this;
            }

            //etc, goes on for ages - tones of imported members 
        }
    }
````


### Adapater-Decorator 
- A pattern that embodies both concepts 


### Multiple Inheritance with Interfaces
- If you have bird and lizard and want Dragon to inherit both of them, you cannot
- Can only inherit multiple interfaces 
- give it the fields for Bird and Lizard (in the example they are just newed up, but in reality they would be dependency injected)
- right click => generate => delegating members => select all (and theyre automatically added)
- Lets say lizard and bird both have a Weight field, what happens to dragon? 
  - if you generate delegating members, you get both the lizard and the bird weight 
  - you need to implement it yourself 
- If the weight is linked to other attributes, have to define the setter
````c#
public interface IBird
{
    void Fly();

    public int Weight { get; set; }
}

public class Bird : IBird
{
    public void Fly()
    {
        Console.WriteLine($"Soaring in the sky with weight {Weight}");
    }

    public int Weight { get; set; }
}

public interface ILizard
{
    void Crawl();
    public int Weight { get; set; }
}

public class Lizard : ILizard
{
    public void Crawl()
    {
        Console.WriteLine($"Crawling in the dirt with {Weight}");
    }

    public int Weight { get; set; }
}

public class Dragon : IBird, ILizard //cannot inherit from both bird and lizard, so use decorator pattern
{
    private Bird bird = new Bird();
    private Lizard lizard = new Lizard();
    
    public int Weight
    {
        get { return Weight; }
        set //need to do this for a shared field... one of the 'features' of this pattern
        {
            lizard.Weight = value;
            bird.Weight = value;
        }
    }
    
    public void Fly()
    {
        bird.Fly();
    }
    
    public void Crawl()
    {
        lizard.Crawl();
    }
}
````


### Multiple Inheritance with 'Default Interface Members' (new C# has this)
````c#
public interface ICreature
{
    int Age { get; set; }
}

public interface IBird2 : ICreature
{
    public void Fly()
    {
        if (Age >= 10)
            Console.WriteLine("I am flying");
    }
}

public interface ILizard2 : ICreature
{
    public void Crawl()
    {
        if (Age >= 10)
            Console.WriteLine("I am crawling");
    }
}

public class Organism {}

public class Dragon2 : Organism, IBird2, ILizard2
{
    public int Age { get; set; }
}

// inheritance (can't inherit from multiple classes, i.e. already inherits from organism)
// SmartDragon(Dragon) - typical decorator design pattern 
// if u just want to add behaviour: 
// extension methods can add behavior - option 1 
// c# 8 default interface methods - option 2

  class Program
  {
      static void Main(string[] args)
      {
          var d = new Dragon2 {Age = 5};
          // we don't have access to Fly or Crawl, the methods are on the interfaces not the class
          
          ((ILizard2)d).Crawl(); //if we cast it, we have access it
          
          if (d is IBird2 bird)
              bird.Fly(); //can also do this 
          
          //to do this method you need the casts!
          
          //this is more intrusitve than adding extension methods: extension methods don't require you edit the class itself
      }
}
````


- There is also a "dynamic decorator" composition
- There is also a "static decorator" composition
- Decorator in dependency injection (lecture 62)


### Summary Decorator:
- A decorator keeps the reference fo the decorated object(s)
- May or may not proxy over calls ( may or may not replicate API - might not need them all )
  - use rider to generate delegated members
- Decorator also exists in a static variation in C# (layering of decorators)
  - very limited due to inability to inherit from type parameters
  - X<Y<Foo>>
  - not recommended in c#


---


# Facade 
- Exposing several components through a single interface
- Motivation: 
  - Balancing complexity and presentation / usability
  - typical home
    - has many subsystems (electrical, sanitation)
    - complex internal structure (e.g. multi layered floor)
    - end user is not exposed to internals 
  - Same with software - consumers just want something simple to work with
    - many systems working to provide flexibility but...
    - API consumers want it to 'just work'


- Facade: Provides a simple, east to understand / user interface over a large and sophisticated body of code 


### Example
- Quite boring to show, really just depends on what you want to show through the facade
- Uses some long winded example, of Console.WriteFormat instead of writeline ... 


### Summary Facade
- Build a facade to provide a simplified API over a set of classes
- May wish to (optionally) expose internals through the facade
- May allow users to 'escalate' to use more complex APIs if thats what they wish to do 


---


# Flyweight design pattern
- Space optimization
- Motivation: 
  - Avoid redundancy when storing data
  - E.g. MMORPG games 
    - plenty of users with identical first/last names, i.e. "John Smith"
    - no sense in storing these names over and over again
    - store a list of names and pointers to them (references, indicies)
  - .NET performs string interning, so an identical string is stored only once  - strings are immutable (if you have more than one string of the same, it will point to the same thing )
  - flyweight built into .NET
  - E.g. bold or italic text in the console 
    - don't want each character to have a formatting character
    - operate on ranges (e.g. line number, start / end positions)


- Flyweight: A space optimization technique that lets us use less memory by storing externally the data associated with similar objects.


### Repeating user names (Example)
- He installs this nuget package "dotnetmemoryunit"
- when you run the test, you gotta click "run under dotmemory unit"
````c#
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
````
- Note: Above code didn't actually generate smaller numbers in the tests? In the comments on the video others had this problem too. Author used .NET and we used .NET Core



### Flyweight Summary
- Store common data externally
- Define the idea of 'ranges' on homogeneous collections and store data related to those ranges 
- .NET string interning is the flyweight pattern 


---


# Proxy
- An interface for accessing a particular resource
- Motivation:
  - You are calling foo.Bar()
  - This assumes that foo is in the same process as Bar()
  - What if, later on, you want to put all Foo-related operations into a separate proces
    - Can we avoid changing your code?
  - Proxy to the rescue!
    - Same interface, entirely different behavior 
  - This is called a communication proxy (invokations only appear to be local)
    - Other types of proxys too: logging, virtual, guarding...


- Proxy: Providing a class that functions as an interface to a particular resource. That resource may be remote, expensive to construct, or may require logging or some other added functionality.



Types shown in the course:
- Protection Proxy
  - i.e. Check if current user has the access rights to call a particular method or rest resource etc
- Property Proxy
  - i.e. if the value is already 10 and you try to assign it 10, we do nothing
- Value Proxy
  - different to property property, typically constructed over a primitive type. why? you want stronger typing, i.e. price instead of int 
- Composite Proxy: SoA/AoS
  - composite design pattern AND proxy pattern.
  - usually just in game dev, or anyone processing large numbers of complex data
- Composite Proxy with Array-backed properties
- Dynamic Proxy for logging
  - static is faster and preferred proxy as opposed to dynamic 
  - dynamic proxy is constructed at runtime (but has performance costs)
- Proxy vs Decorator 
  - Proxy provides an identitcal interface; decorator provides an enhanced interface (adds additional functionality)
  - Decorator typically reference what it is decorating (ie taking the thing its deocrating in its constructor); proxy doesnt have to
  - Proxy might not even be working with a materialized object (just an interface)
- ViewModel
  - for UI stuff


### Protection Proxy
- Check if current user has the access rights to call a particular method or rest resource etc. 
- You replicate the members (similar to decorator) but you don't add new members, you add new functionality to existing members 
````c#
public interface ICar
{
    void Drive();
}

public class Car : ICar
{
    public void Drive()
    {
        Console.WriteLine("Car is being driven");
    }
}

public class Driver
{
    public int Age { get; set; }

    public Driver(int age)
    {
        this.Age = age;
    }
}

public class CarProxy : ICar
{
    public Driver driver { get; set; }
    private Car car = new Car();
    public CarProxy(Driver driver)
    {
        this.driver = driver;
    }
    
    public void Drive()
    {
        if (driver.Age >= 16)
            car.Drive();
        else
            Console.WriteLine("too young");
    }
}

class Program
{
    static void Main(string[] args)
    {
        ICar car = new CarProxy(new Driver(22));
        car.Drive();
        
    }
}
````


### Proxy Summary
- A proxy has the same interface as the underlying object
- To create a proxy, simply replicate the existing interface of an object
- Add relevant functionality to the redefined member functions 
- Different proxies (communication, logging, caching, etc) have completely different behaviors 


---