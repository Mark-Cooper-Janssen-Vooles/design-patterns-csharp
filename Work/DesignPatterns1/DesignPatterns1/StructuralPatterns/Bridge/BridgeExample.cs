// using System;
// using Autofac;
// using DesignPatterns1.SOLID;
//
// namespace DesignPatterns1.StructuralPatterns.Bridge
// {
//     public interface IRenderer
//     {
//         void RenderCircle(float radius);
//     }
//     
//     public class VectorRenderer : IRenderer
//     {
//         public void RenderCircle(float radius)
//         {
//             Console.WriteLine($"Drawing a cirlce of {radius}");
//         }
//     }
//     
//     public class RasterRenderer : IRenderer
//     {
//         public void RenderCircle(float radius)
//         {
//             Console.WriteLine($"Drawing pixels fr circle with radius {radius}");
//         }
//     }
//
//     public abstract class Shape
//     {
//         protected IRenderer renderer;
//
//         protected Shape(IRenderer renderer)
//         {
//             this.renderer = renderer;
//         }
//
//         public abstract void Draw();
//         public abstract void Resize(float factor);
//     }
//
//     public class Circle : Shape
//     {
//         private float radius;
//
//         public Circle(IRenderer renderer, float radius) : base(renderer)
//         {
//             this.radius = radius;
//         }
//
//         public override void Draw()
//         {
//             renderer.RenderCircle(radius);
//         }
//
//         public override void Resize(float factor)
//         {
//             radius *= factor;
//         }
//     }
//
//     // class Program
//     // {
//     //     static void Main(string[] args)
//     //     {
//     //         //var renderer = new RasterRenderer();
//     //         // var renderer = new VectorRenderer();
//     //         // var circle = new Circle(renderer, 5);
//     //         //
//     //         // circle.Draw();
//     //         // circle.Resize(2);
//     //         // circle.Draw();
//     //
//     //         var cb = new ContainerBuilder();
//     //         cb.RegisterType<VectorRenderer>().As<IRenderer>(); //whenever anyone asks for an IRenderer, inject a vectorRenderer
//     //         cb.Register((C, p) =>
//     //             new Circle(C.Resolve<IRenderer>(),
//     //                 p.Positional<float>(0)));
//     //
//     //         using (var c = cb.Build())
//     //         {
//     //             var circle = c.Resolve<Circle>(
//     //                 new PositionalParameter(0, 5.0f)
//     //             );
//     //             circle.Draw();
//     //             circle.Resize(2.0f);
//     //             circle.Draw();
//     //         }
//     //     }
//     // }
// }