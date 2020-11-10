using System;

namespace DesignPatterns1.StructuralPatterns.Proxy
{
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

    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         ICar car = new CarProxy(new Driver(22));
    //         car.Drive();
    //         
    //     }
    // }
}