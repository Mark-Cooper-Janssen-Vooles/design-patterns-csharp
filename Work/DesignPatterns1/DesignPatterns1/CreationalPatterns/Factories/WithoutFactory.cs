using System;

namespace DesignPatterns1.CreationalPatterns.Factories
{
    public enum CoordinateSystem
    {
        Cartesian, 
        Polar
    }
    
    public class Point
    {
        private double x, y;

        //if its polar there is no indication that a is rho and b is theta, so you will need to add XML comments...
        //adds additional mental complexity
        /// <summary>
        /// Initializes a point from EITHER cartesian or polar
        /// </summary>
        /// <param name="a">x if cartesian, rho if polar</param>
        /// <param name="b">y if carterisan, theta if polar</param>
        /// <param name="system"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Point(double a, double b, CoordinateSystem system = CoordinateSystem.Cartesian) //cartesian by default 
        {
            switch (system)
            {
                case CoordinateSystem.Cartesian:
                    x = a;
                    y = b;
                    break;
                case CoordinateSystem.Polar:
                    x = a * Math.Cos(b);
                    y = a * Math.Sin(b);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    //class Program
    //{
        // static void Main(string[] args)
        // {
        //     
        // }
    //}
}