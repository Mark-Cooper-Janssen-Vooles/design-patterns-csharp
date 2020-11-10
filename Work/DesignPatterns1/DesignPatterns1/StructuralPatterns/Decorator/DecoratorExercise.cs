namespace DesignPatterns1.StructuralPatterns.Decorator
{
    public class Bird3
    {
        public int Age { get; set; }
      
        public string Fly()
        {
            return (Age < 10) ? "flying" : "too old";
        }
    }

    public class Lizard3
    {
        public int Age { get; set; }
      
        public string Crawl()
        {
            return (Age > 1) ? "crawling" : "too young";
        }
    }

    public class Dragon3 // no need for interfaces
    {
        private Bird3 bird = new Bird3();
        private Lizard3 lizard = new Lizard3();
        
        public int Age
        {
            get { return Age; }
            set 
            {
                bird.Age = value;
                lizard.Age = value;
            }
        }

        public string Fly()
        {
            return bird.Fly();
        }

        public string Crawl()
        {
            return lizard.Crawl();
        }
    }
    
    
}