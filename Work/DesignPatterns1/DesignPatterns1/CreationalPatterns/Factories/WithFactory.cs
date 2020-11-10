using System;

namespace DesignPatterns1.CreationalPatterns.Factories
{
    public class Point2
    {
        public static Point2 NewCartesianPoint(double x, double y)
        {
            return new Point2(x, y);
        }
        
        public static Point2 NewPolarPoint(double rho, double theta)
        {
            return new Point2(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }

        private double x, y;

        private Point2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var point = Point2.NewPolarPoint(1, Math.PI / 2);
    //         Console.WriteLine(point);
    //     }
    // }
}