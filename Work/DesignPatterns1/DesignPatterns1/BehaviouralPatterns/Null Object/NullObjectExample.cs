using System;
using Autofac;
using JetBrains.Annotations;

namespace DesignPatterns1.StructuralPatterns.Null_Object
{
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

    //
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var cb = new ContainerBuilder();
    //         cb.RegisterType<BankAccount2>();
    //         cb.RegisterType<NullLog>().As<ILog>(); //this makes it OK.
    //         using (var c = cb.Build())
    //         {
    //             var ba = c.Resolve<BankAccount2>();
    //         }
    //     }
    // }
}