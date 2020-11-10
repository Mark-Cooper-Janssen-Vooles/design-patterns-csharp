using System;

namespace DesignPatterns1.BehaviouralPatterns.Observer
{
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
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var person = new Person();
    //         person.FallsIll += CallDoctor; //when a person falls ill, this is how you call an event
    //         person.CatchACold();
    //
    //         person.FallsIll -= CallDoctor; //this removes it from the events 
    //     }
    //
    //     private static void CallDoctor(object? sender, FallsIllEventArgs e)
    //     {
    //         Console.WriteLine($"A doctor has been called to {e.Address}");
    //     }
    // }
}