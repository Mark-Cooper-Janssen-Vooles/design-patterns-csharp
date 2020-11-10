using System;

namespace DesignPatterns1.CreationalPatterns.Factories
{
    public class Point4
    {
        private double x, y;

        public Point4(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
        }
        
        public static class Factory
        {
            public static Point4 NewCartesianPoint(double x, double y)
            {
                return new Point4(x, y);
            }

            public static Point4 NewPolarPoint(double rho, double theta)
            {
                return new Point4(rho * Math.Cos(theta), rho * Math.Sin(theta));
            }
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var point = Point4.Factory.NewPolarPoint(1, Math.PI / 2);
    //         Console.WriteLine(point);
    //     }
    // }
}