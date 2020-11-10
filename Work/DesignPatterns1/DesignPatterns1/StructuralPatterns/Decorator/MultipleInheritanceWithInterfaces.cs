using System;

namespace DesignPatterns1.StructuralPatterns.Decorator
{
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
}