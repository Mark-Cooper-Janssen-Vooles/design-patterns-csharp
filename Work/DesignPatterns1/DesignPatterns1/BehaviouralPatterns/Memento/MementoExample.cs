using System;

namespace DesignPatterns1.StructuralPatterns.Memento
{
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

    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var ba = new BankAccount1(100);
    //         var m1 = ba.Deposit(50); // 150
    //         var m2 = ba.Deposit(25); //175
    //         Console.WriteLine(ba);
    //
    //         ba.Restore(m1); //roll back to 150
    //         Console.WriteLine(ba);
    //
    //         ba.Restore(m2);
    //         Console.WriteLine(ba);
    //     }
    // }
}