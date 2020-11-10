using System;

namespace DesignPatterns1.CreationalPatterns.Factories
{

    public static class PointFactory
    {
        public static Point3 NewCartesianPoint(double x, double y)
        {
            return new Point3(x, y);
        }

        public static Point3 NewPolarPoint(double rho, double theta)
        {
            return new Point3(rho * Math.Cos(theta), rho * Math.Sin(theta));
        }
    }
    
    public class Point3
    {
        private double x, y;

        public Point3(double x, double y)
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
    //         var point = PointFactory.NewPolarPoint(1, Math.PI / 2);
    //         Console.WriteLine(point);
    //     }
    // }
}