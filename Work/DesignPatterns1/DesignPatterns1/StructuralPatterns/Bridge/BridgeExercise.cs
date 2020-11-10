// using System;
// using DotNetDesignPatternDemos.Structural.Bridge.Coding.Exercise;
//
// namespace DotNetDesignPatternDemos.Structural.Bridge
// {
//     namespace Coding.Exercise
//     {
//         public interface IRenderer
//         {
//             string WhatToRenderAs { get; }
//         }
//
//         public abstract class Shape
//         {
//             public string Name { get; set; }
//             protected IRenderer renderer;
//
//             public Shape(IRenderer renderer)
//             {
//                 this.renderer = renderer;
//             }
//
//             public override string ToString()
//             {
//                 return $"Drawing {Name} as {renderer.WhatToRenderAs}";
//             }
//         }
//
//         public class Triangle : Shape
//         {
//             public Triangle(IRenderer renderer) : base(renderer)
//             {
//                 Name = "Triangle";
//             }
//         }
//
//         public class Square : Shape
//         {
//             public Square(IRenderer renderer) : base(renderer)
//             {
//                 Name = "Square";
//             }
//         }
//
//         public class VectorRenderer : IRenderer
//         {
//             public string WhatToRenderAs => "pixels";
//         }
//     }
//
//     public class RasterRenderer : IRenderer
//     {
//         public string WhatToRenderAs => "lines";
//     }
// }
//
// class Program
//         {
//             static void Main(string[] args)
//             {
//                 var returns = new Square(new VectorRenderer()).ToString();
//
//                 Console.WriteLine(returns);
//             }
//         }
// }
//
