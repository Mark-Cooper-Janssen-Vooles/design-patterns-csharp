using System;
using System.Dynamic;

namespace DesignPatterns1.StructuralPatterns.ChainOfResponsibility
{
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


    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var goblin = new Creature("Goblin", 2, 2);
    //         Console.WriteLine(goblin);
    //         
    //         var root = new CreatureModifier(goblin);
    //         //root.Add(new NoBonusesModifier(goblin)); //handle isn't called so linked list dies and other stats wont update
    //
    //         Console.WriteLine("Lets double goblins attack");
    //         root.Add(new DoubleAttackModifier(goblin));
    //
    //         Console.WriteLine("Lets increase goblins defense");
    //         root.Add(new IncreaseDefenseModifier(goblin));
    //
    //         root.Handle(); //takes care of every modifier in the chain
    //
    //         Console.WriteLine(goblin);
    //     }
    // }
}