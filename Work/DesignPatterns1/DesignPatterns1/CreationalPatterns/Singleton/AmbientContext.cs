using System;
using System.Collections.Generic;

namespace DesignPatterns1.CreationalPatterns.Singleton
{
    public class BuildingContext
    {
        public static int WallHeight;
    }
    
    public class Building
    {
        public List<Wall> Walls = new List<Wall>();
    }

    public struct Point
    {
        private int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Wall
    {
        public Point Start, End;
        public int Height;

        public Wall(Point start, Point end)
        {
            Start = start;
            End = end;
            Height = BuildingContext.WallHeight;
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var house = new Building();
    //         //ground, height is 3000
    //         BuildingContext.WallHeight = 3000;
    //         house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0)));
    //         house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000)));
    //         
    //         //1st floor, height is 3500
    //         BuildingContext.WallHeight = 3500;
    //         house.Walls.Add(new Wall(new Point(0, 0), new Point(5000, 0)));
    //         house.Walls.Add(new Wall(new Point(0, 0), new Point(0, 4000)));
    //
    //         //ground, height is 3000
    //         BuildingContext.WallHeight = 3000;
    //         house.Walls.Add(new Wall(new Point(5000, 0), new Point(5000, 4000)));
    //     }
    // }
}