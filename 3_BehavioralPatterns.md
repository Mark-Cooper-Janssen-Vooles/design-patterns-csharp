Contents:
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


---


# Chain of Responsibility
- Sequence of handlers processing an event one after another
- Motivation:
  - Unethical behavior by an employee, who takes the blame?
    - employee
    - manager
    - CEO 
  - You click a graphical element on a form
    - Button handles it, stops further processing
    - Underlying group box 
    - Underlying window 
  - CCG computer game (collectable card game)
    - creature has attack and defense values
    - those can be boosted by other cards
    - Need to look at the chain of boosts etc


- Chain of responsibility: A chain of components who all get a chance to process a command or a query, optionally having default processing implementation and an ability to terminate the processing chain


### Command Query separation
- When we operate on objects, we seperate 
- Command = asking for an action or change (e.g. set your attack value to 2)
- Query = asking for information without changing anything (e.g. give me your attack value)
- CQS = having separate means of sending commands and queries to e.g. direct field access 
- 1 method responsibile for querying, one for commanding. Only do one thing!


### Method Chain
````c#
public class Creature
{
    public string Name;
    public int Attack, Defense;

    public Creature(string name, int attack, int defense)
    {
        Name = name;
        Attack = attack;
        Defense = defense;
    }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Attack)}: {Attack}, {nameof(Defense)}: {Defense}";
    }
}

public class CreatureModifier //facilitates chain of responsibility design pattern, a base class, collection of all modifiers
{
    protected Creature creature;
    protected CreatureModifier next; //linked list

    public CreatureModifier(Creature creature)
    {
        this.creature = creature;
    }

    public void Add(CreatureModifier cm) //add a new modifier to a creature
    {
        if (next != null)
            next.Add(cm);
        else
            next = cm;
    }

    public virtual void Handle() => next?.Handle();
}

public class DoubleAttackModifier : CreatureModifier
{
    public DoubleAttackModifier(Creature creature) : base(creature)
    {
    }

    public override void Handle()
    {
        Console.WriteLine($"Doubling {creature.Name}'s attack");
        creature.Attack *= 2;
        base.Handle();

    }
}

public class IncreaseDefenseModifier : CreatureModifier
{
    public IncreaseDefenseModifier(Creature creature) : base(creature)
    {
    }

    public override void Handle()
    {
        Console.WriteLine($"Increasing {creature.Name}'s defense");
        creature.Defense += 3;
        base.Handle();
    }
}

public class NoBonusesModifier : CreatureModifier
{
    public NoBonusesModifier(Creature creature) : base(creature)
    {
    }

    public override void Handle()
    {
        //nothing here so handle isn't called and linked list is killed
    }
}


class Program
{
    static void Main(string[] args)
    {
        var goblin = new Creature("Goblin", 2, 2);
        Console.WriteLine(goblin);
        
        var root = new CreatureModifier(goblin);
        //root.Add(new NoBonusesModifier(goblin)); //handle isn't called so linked list dies and other stats wont update

        Console.WriteLine("Lets double goblins attack");
        root.Add(new DoubleAttackModifier(goblin));

        Console.WriteLine("Lets increase goblins defense");
        root.Add(new IncreaseDefenseModifier(goblin));

        root.Handle(); //takes care of every modifier in the chain

        Console.WriteLine(goblin);
        
        
    }
}
````


### Broker Chain
- He doesn't like chain of responsibility: 
  - handle permanently modifies the creature
  - prevents us from removing the modifiers later on 
  - have to expose attack and defense to modify them
- Broker chain is intermediatior + chain of responsibility (a better way to do it)


### Chain of Responsibility Summary
- Can be implemented as a chain of references or a centeralized construct (that was the example we used)
- Enlist objects in the chain, possibly controlling their order
- Object removal from chain (e.g. in Dispose())


---


# Command
- "You shall not pass!"
- Motivation:
  - Ordinary c# statements are perishable
    - cannot undo a field / property assignment (i.e. can't unassign or unwrite)
    - cannot directly serialise a sequence of actions (or calls)
  - Want an object that represents an operation
    - X should change its property Y to Z
    - X should do W()
  - Uses: GUI commands (i.e. file => save), multi-level undo/redo, macro recording and more!


- Command: Lets you build an object which represents an instruction to perform a particular action. This command contains all the information necessary for the action to be taken. 


### Command with bank account example
- Want to record all the transactions etc. 
````c#
public class BankAccount
{
    private int balance;
    private int overdraftLimit = -500;

    public void Deposit(int amount)
    {
        balance += amount;
        Console.WriteLine($"Deposited ${amount}, balance is now ${balance}.");
    }

    public void Withdraw(int amount)
    {
        if (balance - amount >= overdraftLimit)
        {
            balance -= amount;
            Console.WriteLine($"Withdraw ${amount}, balance is now ${balance}.");
        }
    }

    public override string ToString()
    {
        return $"{nameof(balance)}: {balance}";
    }
}

public interface ICommand
{
    void Call();
}

public class BankAccountCommand : ICommand
{
    private BankAccount account;
    public enum Action
    {
        Deposit, Withdraw
    }
    private Action action;
    private int amount;

    public BankAccountCommand(BankAccount account, Action action, int amount)
    {
        this.account = account;
        this.action = action;
        this.amount = amount;
    }
    
    public void Call()
    {
        switch (action)
        {
            case Action.Deposit:
                account.Deposit(amount);
                break;
            case Action.Withdraw:
                account.Withdraw(amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        var ba = new BankAccount();
        var commands = new List<BankAccountCommand>
        {
            new BankAccountCommand(ba, BankAccountCommand.Action.Deposit, 100),
            new BankAccountCommand(ba, BankAccountCommand.Action.Withdraw, 50)
        };
        // we now have a list of commands that are stored ^ instead of a list we could store these somewhere, i.e. a db.

        Console.WriteLine(ba);
        
        foreach (var c in commands)
            c.Call();
    }
}
````


### Command Summary
- Encaspulate all details of an operation in a separate object
- Define instruction for applying the command (either in the command itself, or elsewhere)
- Optionally define instructions for undoing the command 
- Can create composite commands (aka macros)


---


# Interpreter 
- "Interpreers are all around us. Even now, in this very room."
- Interpreter design pattern is a separate field of computer science 
- Textual input needs to be processed (into executables etc)
  - e.g. turned into OOP structures 
- Some examples:
  - Programming language compilers, interpreters and IDEs
  - HTML, XML, and similar
  - Numeric expressions (3+4 / 5)
  - Regular expressions
- Turning strings into OOP based structures in a complicated process


- Interpreter: A component that processes structured text data. Does so by turning it into separate lexical tokens (lexing) and then interpreting seqences of said tokens (parsing)


- Note: didn't do any examples of this, seems irrelevant for what i want to do. 


### Interpreter summary
- Barring simpel cases, an interpreter acts in two stages
- Lexing turns text into a set of tokens
- Parsing tokens into meaningful constructs 
- Parsed data can then be traversed (interpeted, transformmed etc)


---


# Iterator
- "How traversal of data structures happens and who makes it happen" 
- Motivation:
  - Iteration (traversal) is a core functionality of various data structures
  - An iterator is a class that facilities the traversal
    - keeps a reference to the current element
    - Knows how to move to a different element
  - Iterator is an implicit construct,
    - i.e. ig you use IEnumerable<T>, .NET builds a state machine around your yield return statements


- Iterator: An object (or, in .NET, a method using IEnumerable<T>) that facilitates the traversal of a data structure


- this section shows us how .NET framework makes the iterator / traversal possible, so gonna skip examples


### Iterator summary
- An iterator specified how you can traverse an object (in order, pre-order, post order)
- An iterator object, unlike a method, cannot be recursive
- Generally, an IEnumerable<T>-returning method is enough 
- Iteration works through duck typing - you need a GetEnumerator() that yields a type that has Current and MoveNext()


---


# Medidator 
- "Facilitates communication between components", by letting components be unaware of each others presence or absense in the system
- Motivation:
  - Components may go in and out of a system at any time (i.e. a chatroom)
    - Chat room participants
    - Players in an MMORPG
  - It makes no sense for them to have direct references to one another
    - these references may go dead at any time
  - Solution: have them all refer to some central component that facilitates communication 


- Mediator: A component that facilitates communication between other components without them necessarily being aware of each other or havng direct (reference) access to each other. 


### Chat room example
- The chatroom acts as a mediatior between everyone, they don't have to be aware of anyone else beforehand (no references required between one another)
````c#
public class PersonM
{
    public string Name;
    public ChatRoom room;
    private List<string> chatLog = new List<string>();

    public PersonM(string name)
    {
        Name = name;
    }

    public void Say(string message)
    {
        room.Broadcast(Name, message);
    }

    public void PrivateMessage(string who, string message)
    {
        room.Message(Name, who, message);
    }

    public void Receive(string sender, string message)
    {
        string s = $"{sender}: '{message}'";
        chatLog.Add(s);
        Console.WriteLine($"[{Name}'s chat session] {s}");
    }
}

public class ChatRoom
{
    private List<PersonM> people = new List<PersonM>();
    
    public void Join(PersonM p)
    {
        string joinMsg = $"{p.Name} joins the chat";
        Broadcast("Room", joinMsg);

        p.room = this;
        people.Add(p);
    }

    public void Broadcast(string source, string message)
    {
        foreach (var p in people)
            if (p.Name != source)
                p.Receive(source, message);
    }

    public void Message(string source, string destination, string message)
    {
        people.FirstOrDefault(p => p.Name == destination)
            ?.Receive(source, message);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var room = new ChatRoom();
        
        var john = new PersonM("John");
        var jane = new PersonM("Jane");

        room.Join(john);
        room.Join(jane);

        john.Say("hi");
        jane.Say("oh, hey john");
        
        var simon = new PersonM("Simon");
        room.Join(simon);
        simon.Say("hi everyone");
        
        jane.PrivateMessage("Simon", "Glad you could join us");
    }
}
````


### Introduction to MediatR
- get the nuget package



### Mediator Summary
- Create the mediator and have each object in the system refer to it
  - e.g. in a field via constructor injection
  - a mediator is usually a singleton
- Mediator engages in bidirectional communication with its connected components
- Mediator has functions the components can call
- Event processing (e.g. Rx) libraries make communication easier to implement 


---


# Memento
- "Keep a memento of an object's state to return to that state" 
- Motivation:
  - An object or system goes through changes
    - e.g. a bank account gets deposits and withdrawls
  - There are different ways of navigating those changes
  - One way is to record every change (command) and teach a command to 'undo' itself
  - Another is to simply save snapshots of the system


- Memento: A token / handle representing the system state. Lets us roll back to the state when the token was generated. May or may not directly expose state information. 


### Memento Example
- You essentially provide an API that returns special tokens 
- Tokens represent the state of the system at different times 
````c#
public class Memento
{
    public int Balance { get;  }
    
    public Memento(int balance)
    {
        this.Balance = balance;
    }
}

public class BankAccount1
{
    private int balance;

    public BankAccount1(int balance)
    {
        this.balance = balance;
    }

    public Memento Deposit(int amount)
    {
        balance += amount;
        return new Memento(balance); //instead of void we return a memento, which keeps the balance at this point in time
    }

    public void Restore(Memento m)
    {
        balance = m.Balance; //you can get the balance of a memento, you just can't set it 
    }

    public override string ToString()
    {
        return $"{nameof(balance)}: {balance}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        var ba = new BankAccount1(100);
        var m1 = ba.Deposit(50); // 150
        var m2 = ba.Deposit(25); //175
        Console.WriteLine(ba);

        ba.Restore(m1); //roll back to 150
        Console.WriteLine(ba);

        ba.Restore(m2);
        Console.WriteLine(ba);
    }
}
````


### Memento Summary
- Mementos are used to roll back states arbitrarily
- A memento is simply a token / handle class with (typically) no functions of its own
- A memento is not required to expose directly the state(s) to which it reverts the system
- Can be used to implement undo/redo 


---


# Null Object
- A behavioral design pattern with no behaviors 
- Motivation: 
  - When component A uses component B, it typically assumse that B is non-null
    - you inject B, but not B? or some Option<B>
    - You do not check for null (?.) on every call (you just do this on events)
  - There is no option of telling A not to use an instance of B
    - Its use is hard-coded
  - Thus, we build a no-op, non-functioning inheritor of B and pass it to A


- Null object: A no-op object that conforms to the required interface, satisfying a dependency requirement of some other object. 


### Null object example
- Why does it exist? 
- Before using null object pattern:
````c#
public interface ILog
{
    void Info(string msg);
    void Warn(string msg);
}

public class ConsoleLog : ILog
{
    public void Info(string msg)
    {
        Console.WriteLine(msg);
    }

    public void Warn(string msg)
    {
        Console.WriteLine("WARNING !!! " + msg);
    }
}

public class BankAccount2
{
    private ILog log;
    private int balance;

    public BankAccount2(ILog log)
    {
        this.log = log;
    }

    public void Deposit(int amount)
    {
        balance += amount;
        log.Info($"Deposited {amount}, balance is now {balance}");
    }
}


class Program
{
    static void Main(string[] args)
    {
        var log = new ConsoleLog();
        var ba = new BankAccount2(log); //if we try pass in null we get a "null reference exception"

        ba.Deposit(100);
    }
}
````
- After using null object pattern:
````c#
public interface ILog
{
    void Info(string msg);
    void Warn(string msg);
}

public class ConsoleLog : ILog
{
    public void Info(string msg)
    {
        Console.WriteLine(msg);
    }

    public void Warn(string msg)
    {
        Console.WriteLine("WARNING !!! " + msg);
    }
}

public class BankAccount2
{
    private ILog log;
    private int balance;

    public BankAccount2([CanBeNull] ILog log) //uses the jetbrains.annotations nuget package
    {
        this.log = log;
    }

    public void Deposit(int amount)
    {
        balance += amount;
        log?.Info($"Deposited {amount}, balance is now {balance}"); //need to "?."
    }
}


class Program
{
    static void Main(string[] args)
    {
        var log = new ConsoleLog();
        var ba = new BankAccount2(null); 

        ba.Deposit(100);
    }
}
````
- The above requires 'upfront design' (the null check) and the DI container doesn't like that you're passing it null
- Null object pattern solves this by implementing a log that does absolutely nothing
- You simply make an object which conforms to the interface that you need, but you leave out the members (leave them blank)
````c#
public interface ILog
{
    void Info(string msg);
    void Warn(string msg);
}

public class ConsoleLog : ILog
{
    public void Info(string msg)
    {
        Console.WriteLine(msg);
    }

    public void Warn(string msg)
    {
        Console.WriteLine("WARNING !!! " + msg);
    }
}

public class BankAccount2
{
    private ILog log;
    private int balance;

    public BankAccount2([CanBeNull] ILog log) //uses the jetbrains.annotations nuget package
    {
        this.log = log;
    }

    public void Deposit(int amount)
    {
        balance += amount;
        log?.Info($"Deposited {amount}, balance is now {balance}"); //need to "?."
    }
}

//i.e. this is the object conforming to the interface:
public class NullLog : ILog
{
    public void Info(string msg)
    {
    }

    public void Warn(string msg)
    {
    }
}


class Program
{
    static void Main(string[] args)
    {
        var cb = new ContainerBuilder();
        cb.RegisterType<BankAccount2>();
        cb.RegisterType<NullLog>().As<ILog>(); //this makes it OK.
        using (var c = cb.Build())
        {
            var ba = c.Resolve<BankAccount2>();
        }
    }
}
````


### Null Object Summary
- it isn't always safe to make a null object
- Implement the required interface
- Rewrite the methods with empty bodies
  - if method is non-void, return default(T)
  - if these values are never used, you are in trouble
- Supply an instance of Null Object in place of actual object
- Dynamic construction possible
  - with associated performance implications 


---


# Observer
- "Build right into c# / .NET right?"
- Motivation:
  - We need to be informed when certain things happen
    - objects property changes
    - object does something
    - some external event occurs
  - We want to listen to events and notified when they occur
  - Built into C# with the event keyword 
    - but then what is this IObservable<T>/IObserver<T> for?
    - What about INotifyPropertyChanging/Changed?
    - And what are BindingList<T>/ObservableCollection<T>?


- Observer: An observer is an object that wishes to be informed about events happening in the system. The entity generating the events is an observable. 


### Observer example 
- All about being informed when something happens
- Often times this is used more for UI in .NET 
- Observer pattern incorporated directly into C# using events. 
````c#
public class FallsIllEventArgs
{
    public string Address;
}
public class Person
{
    //if a person falls ill, you may want to call a doctor 
    public event EventHandler<FallsIllEventArgs> FallsIll;

    public void CatchACold()
    {
        //if no one has subscribed you'll get a null ref exception, so use a ?
        //in this case, CallDoctor has subscribed
        FallsIll?.Invoke(this, new FallsIllEventArgs {Address = "123 fake street"}); //this is what invokes call doctor
    }
}

class Program
{
    static void Main(string[] args)
    {
        var person = new Person();
        person.FallsIll += CallDoctor; //when a person falls ill, this is how you call an event
        person.CatchACold();

        person.FallsIll -= CallDoctor; //this removes it from the events 
    }

    private static void CallDoctor(object? sender, FallsIllEventArgs e)
    {
        Console.WriteLine($"A doctor has been called to {e.Address}");
    }
}
````


### Observer Summary
- Observer is an intrusive approach; an observable must provide an event to subscribe to (it must make notifcations to itself to provide events or some sort of hooks for external components to subscribe whats going on within it)
- Special care must be taken to prevent issues in multithreaded scenarios
- .NET comes with observable collections 
- IObserver<T>/IObservable<T> are used in stream processing (Reactive Extensions)


---


# State 
- "Fun with finite state machines" 
- Most objects have state in their fields.. why is this a seperate pattern?
- Motivation:
  - Consider an ordinary telephone
  - What you do with it depeneds on the state of the phone/line
    - if its ringing or you want to make a call, you need to pick it up
    - phone must be off the hook to talk/make a call
    - if you try calling someone, and its busy, you put the handset down
  - Changes in state can be explicit or in response to event (Observer pattern)


- State pattern: A pattern in which the objects behaviour is determined by its state. An object transitions from one state to another (something needs to trigger a transition)
  - A formalised construct which manages state and transitions is called a state machine 


### Classic State Example
- polymorphism, the offstate inherits from state 
- looks super confusing, theres gotta be easier ways to do it than this? lol a switch? there is a 'switch-based state machine' 
````c#
public class Switch
{
    public State State = new OffState();

    public void On()
    {
        State.On(this); }

    public void Off()
    {
        State.Off(this);
    }
}

public abstract class State
{
    public virtual void On(Switch sw)
    {
        Console.WriteLine("Light is already on.");
    }

    public virtual void Off(Switch sw)
    {
        Console.WriteLine("Light is already off.");
    }
}

public class OnState : State
{
    public OnState()
    {
        Console.WriteLine("Light turned on.");
    }

    public override void Off(Switch sw)
    {
        Console.WriteLine("Turning light off...");
        sw.State = new OffState();
    }
}

public class OffState : State
{
    public OffState()
    {
        Console.WriteLine("Light turned off."); 
    }

    public override void On(Switch sw)
    {
        Console.WriteLine("Turning light on...");
        sw.State = new OnState();
    }
}

class Program
{
    static void Main(string[] args)
    {
        var ls = new Switch();
        ls.On();
        ls.Off();
        ls.Off();
    }
}
````
- He says its wasteful 
- this is just a historic and academic demo. better to sue the other examples 


### State summary
- Given sufficient complexity, it pays to formally define possible states and events/triggers
- Can define
  - state entry/exit behaviors
  - Action when a particular event causes a transition
  - Guard conditions enabling/disabling a transition
  - Default action when no transitions are found for an evenet

 
---


# Stratergy (also known as Policy)
- System behavior partially specified at runtime (and allowed to be augmented later on)
- Motivation
  - Many algorithms can be decomposed into higher and lower-level parts
  - Making tea can be decomposed into
    - the process of making a hot beverage (boil water, pour into cup); and
    - tea-specific things (put teabag into water)
  - The high-level algorithm can then be reused for making coffee or hot chocolate 
    - supported by beverage specific strategies (this is where the stratery pattern comes into it)


- Strategy pattern: Enables the exact behavior of a system to be selected either at run-time (dynamic) or compile-time (static).
  - also known as a policy (esp in the C++ world)


### Dynamic Strategy
- Very simple, all it does is takes away a part of an algorithm and says that you can substitute different parts of that algorithm
````c#
public enum OutputFormat
{
    Markdown,
    Html
}

public interface IListStrategy
{
    void Start(StringBuilder sb);
    void End(StringBuilder sb);
    void AddListItem(StringBuilder sb, string item);
}

public class HtmlListStrategy : IListStrategy
{
    public void Start(StringBuilder sb)
    {
        sb.AppendLine("<ul>");
    }

    public void End(StringBuilder sb)
    {
        sb.AppendLine("</ul>");
    }

    public void AddListItem(StringBuilder sb, string item)
    {
        sb.AppendLine($"  <li>{item}</>");
    }
}

public class MarkdownListStrategy : IListStrategy
{
    public void Start(StringBuilder sb)
    {

    }

    public void End(StringBuilder sb)
    {
        
    }

    public void AddListItem(StringBuilder sb, string item)
    {
        sb.AppendLine($"  * {item}");
    }
}

public class TextProcessor
{
    private StringBuilder sb = new StringBuilder();
    private IListStrategy ListStrategy;

    public void SetOutputFormat(OutputFormat format)
    {
        switch (format)
        {
            case OutputFormat.Markdown:
                ListStrategy = new MarkdownListStrategy();
                break;
            case OutputFormat.Html:
                ListStrategy = new HtmlListStrategy();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    public void AppendList(IEnumerable<string> items)
    {
        ListStrategy.Start(sb);
        foreach (var item in items)
            ListStrategy.AddListItem(sb, item);
        ListStrategy.End(sb);
    }

    public StringBuilder Clear()
    {
        return sb.Clear();
    }

    public override string ToString()
    {
        return sb.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        var tp = new TextProcessor();
        tp.SetOutputFormat(OutputFormat.Markdown);
        tp.AppendList(new[] { "foo", "bar", "baz"});
        Console.WriteLine(tp);

        tp.Clear();
        
        tp.SetOutputFormat(OutputFormat.Html);
        tp.AppendList(new[] { "foo", "bar", "baz"});
        Console.WriteLine(tp);
    }
}
````


### Strategy Summary
- Define an algorithm at a high level
- Define the interface you expect each strategy to follow
- Provide for either dynamic or static composition of strategy in the overall algorithm 


---


# Template Method 
- "A high-level blueprint for an algorithm to be completed by inheritors"
- Motivation: 
  - similar to strategy pattern 
  - Algorithms can be decomposed into parts + specifics
  - Strategy pattern does this through composition
    - high-level algorithm uses an interface
    - concrete implementations implement the interface
  - Template method does the same thing through inheritance
    - overall algorithm makes use of abstract member (typically an abstract base class)
    - Inherits override the abstract members 
    - Parent template method invoked


- Template method: Allows us to define the 'skeleton' of the algorithm, with concrete implementations defined in subclasses.


### Template Method Example
- You define a high-level description of some algorithm where you expect the inheritors to fill-in certain abstract members
````c#
//i.e most games have a start, a turn, and output for winner 
//can make a template method to describe this process
public abstract class Game
{
    public void Run()
    {
        Start();
        while (!HaveWinner)
            TakeTurn();
        Console.WriteLine($"Player {WinningPlayer} wins.");
    }

    protected int CurrentPlayer;
    protected readonly int numberOfPlayers;

    protected Game(int numberOfPlayers)
    {
        this.numberOfPlayers = numberOfPlayers;
    }

    protected abstract void Start(); //inheritor needs to fill this in when the game starts
    protected abstract void TakeTurn();
    protected abstract bool HaveWinner { get; }
    protected abstract int WinningPlayer { get; }
}

public class Chess : Game
{
    public Chess() : base(2)
    {
    }

    protected override void Start()
    {
        Console.WriteLine($"Starting a game of chess with {numberOfPlayers} players.");
    }

    protected override void TakeTurn()
    {
        Console.WriteLine($"Turn {turn++} taken by layer {CurrentPlayer}.");
        CurrentPlayer = (CurrentPlayer + 1) % numberOfPlayers;
    }

    protected override bool HaveWinner => turn == maxTurns;
    protected override int WinningPlayer => CurrentPlayer;

    private int turn = 1;
    private int maxTurns = 10;
}

class Program
{
    static void Main(string[] args)
    {
        var chess = new Chess();
        chess.Run(); //it'll call Run in the Game class, but because all the functions in run are abstract, they will call the correct ones in the chess class.
    }
}
````


### Summary
- Define an algorithm at a high level (typically abstract class with at least one abstract members)
- Define constituent parts as abstract methods/properties
- Inherit the algorithm class, providing necessary overrides


---


# Visitor
- "Typically a tool for structure traversal rather than anything else"
- Motivation:
  - Need to define a new operation (a new method) on an entire class hierarchy
    - e.g. make a document model printable to HTML/Markdown
  - Do not want to keep modifying every class in the hierarchy 
  - Need access to the non-common aspects of classes in the hierarchy 
    - i.e. an extension method wont do
  - Create an external component to handle rendering 
    - But avoid type checks 
  

- Visitor pattern: A pattern where a component (visitor) is allowed to traverse the entire inheritance hierarchy. Implemented by propagating a single visit() (typically called visit()) method throughout the entire hierarchy. It allows 'double dispatch'.


- Dispatch
  - Which function to call?
  - Single dispatch: depends on name of request and type of receiver 
  - Double dispatch: depends on name of request and type of two receivers (type of visitor, type of element being visited)


### Intrusive Expression Printing
- Visitior pattern is all about adding additional functionality when the hierachys are already set and you cant modify the members themselves
````c#
public abstract class Expression
{
    public abstract void Print(StringBuilder sb);
}

public class DoubleExpression : Expression
{
    private double value;

    public DoubleExpression(double value)
    {
        this.value = value;
    }

    public override void Print(StringBuilder sb)
    {
        sb.Append(value);
    }
}

public class AdditionExpression : Expression
{
    private Expression left, right;

    public AdditionExpression(Expression right, Expression left)
    {
        this.right = right;
        this.left = left;
    }

    public override void Print(StringBuilder sb)
    {
        sb.Append("(");
        left.Print(sb);
        sb.Append("+");
        right.Print(sb);
        sb.Append(")");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var e = new AdditionExpression(
            new DoubleExpression(1),
            new AdditionExpression(
                new DoubleExpression(2),
                new DoubleExpression(3)
                ));
        
        var sb = new StringBuilder();
        e.Print(sb);
        Console.WriteLine(sb);
    }
}
````


### Reflection-based Printing
- have a requirement of adding functionality to a hierachy of types 
- If none of the types have a print functionality in it...
- One approach is making a separate component which uses reflection
- End result is you'd have a massive if or switch statement
````c#
public abstract class Expression
{
    //public abstract void Print(StringBuilder sb);
}

public class DoubleExpression : Expression
{
    public double Value;

    public DoubleExpression(double value)
    {
        this.Value = value;
    }
    
}

public class AdditionExpression : Expression
{
    public Expression Left;
    public Expression Right;

    public AdditionExpression(Expression right, Expression left)
    {
        this.Right = right;
        this.Left = left;
    }
    
}

public static class ExpressionPrinter
{
    public static void Print(Expression e, StringBuilder sb)
    {
        if (e is DoubleExpression de)
        {
            sb.Append(de.Value);
        } else if (e is AdditionExpression ae)
        {
            sb.Append("(");
            Print(ae.Left, sb);
            sb.Append("+");
            Print(ae.Right, sb);
            sb.Append(")");
        }
            
    }
}

class Program
{
    static void Main(string[] args)
    {
        var e = new AdditionExpression(
            new DoubleExpression(1),
            new AdditionExpression(
                new DoubleExpression(2),
                new DoubleExpression(3)
                ));
        
        var sb = new StringBuilder();
        ExpressionPrinter.Print(e, sb);
        Console.WriteLine(sb);
    }
}
````
- Using a dictionary: 
````c#
using DictType = Dictionary<Type, Action<Expression, StringBuilder>>;

public abstract class Expression
{
}

public class DoubleExpression : Expression
{
    public double Value;

    public DoubleExpression(double value)
    {
        this.Value = value;
    } 
}

public class AdditionExpression : Expression
{
    public Expression Left;
    public Expression Right;

    public AdditionExpression(Expression right, Expression left)
    {
        this.Right = right;
        this.Left = left;
    }
    
}

public static class ExpressionPrinter
{
    private static DictType actions = new DictType
    {
        [typeof(DoubleExpression)] = (e, sb) =>
        {
            var de = (DoubleExpression) e;
            sb.Append(de.Value);
        },
        [typeof(AdditionExpression)] = (e, sb) =>
        {
            var ae = (AdditionExpression) e;
            sb.Append("(");
            Print(ae.Left, sb);
            sb.Append("+");
            Print(ae.Right, sb);
            sb.Append(")");
        }
    };

    public static void Print(Expression e, StringBuilder sb)
    {
        actions[e.GetType()](e, sb); 
    }
}

class Program
{
    static void Main(string[] args)
    {
        var e = new AdditionExpression(
            new DoubleExpression(1),
            new AdditionExpression(
                new DoubleExpression(2),
                new DoubleExpression(3)
                ));
        
        var sb = new StringBuilder();
        ExpressionPrinter.Print(e, sb);
        Console.WriteLine(sb);
    }
}
````


### Classic Visitor (Double Dispatch)
- you're okay to modify the hierachy just once
````c#
public interface IExpressionVisitor
{
    void Visit(DoubleExpression de);
    void Visit(AdditionExpression ae);
}

public abstract class Expression
{
    public abstract void Accept(IExpressionVisitor visitor);
}

public class DoubleExpression : Expression
{
    public double Value;

    public DoubleExpression(double value)
    {
        this.Value = value;
    }

    public override void Accept(IExpressionVisitor visitor)
    {
        // using double dispatch
        visitor.Visit(this);
    }
}

public class AdditionExpression : Expression
{
    public Expression Left;
    public Expression Right;

    public AdditionExpression(Expression right, Expression left)
    {
        this.Right = right;
        this.Left = left;
    }

    public override void Accept(IExpressionVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class ExpressionPrinter : IExpressionVisitor
{
    private StringBuilder sb = new StringBuilder();
    
    public void Visit(DoubleExpression de)
    {
        sb.Append(de.Value);
    }

    public void Visit(AdditionExpression ae)
    {
        sb.Append("(");
        ae.Left.Accept(this);
        sb.Append("+");
        ae.Right.Accept(this);
        sb.Append(")");
    }

    public override string ToString()
    {
        return sb.ToString();
    }
}

public class ExpressionCalculator : IExpressionVisitor
{
    public double Result;
    
    public void Visit(DoubleExpression de)
    {
        Result = de.Value;
    }

    public void Visit(AdditionExpression ae)
    {
        ae.Left.Accept(this);
        var a = Result;
        ae.Right.Accept(this);
        var b = Result;
        Result = a + b;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var e = new AdditionExpression(
            new DoubleExpression(1),
            new AdditionExpression(
                new DoubleExpression(2),
                new DoubleExpression(3)
                ));
        
        var ep = new ExpressionPrinter();
        ep.Visit(e);
        Console.WriteLine(ep);
        
        var calc = new ExpressionCalculator();
        calc.Visit(e);
        Console.WriteLine($"{ep} = {calc.Result}");
    }
}
````


### Visitor Summary
- Propagate an accept(Visitor v) method throughout the entire hierarchy
- Create a visitor with Visit(Foo), Visit(Bar), ... for each element in the hierarchy
- Each accept() simply calls visitor.Visit(this)
- Using dynamic, we can invoke right overload based on argument type alone (dynamic dispatch) - but we lose some performance


---