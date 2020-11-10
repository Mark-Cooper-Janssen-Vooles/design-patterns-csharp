using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns1.BehaviouralPatterns.Visitor
{
    public interface IExpressionVisitor
    {
        void Visit(DoubleExpression de);
        void Visit(AdditionExpression ae);
    }
    
    public abstract class Expression
    {
        public abstract void Accept(IExpressionVisitor visitor);
    }
    
    public class DoubleExpression : Expression
    {
        public double Value;

        public DoubleExpression(double value)
        {
            this.Value = value;
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            // using double dispatch
            visitor.Visit(this);
        }
    }
    
    public class AdditionExpression : Expression
    {
        public Expression Left;
        public Expression Right;

        public AdditionExpression(Expression right, Expression left)
        {
            this.Right = right;
            this.Left = left;
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ExpressionPrinter : IExpressionVisitor
    {
        private StringBuilder sb = new StringBuilder();
        
        public void Visit(DoubleExpression de)
        {
            sb.Append(de.Value);
        }

        public void Visit(AdditionExpression ae)
        {
            sb.Append("(");
            ae.Left.Accept(this);
            sb.Append("+");
            ae.Right.Accept(this);
            sb.Append(")");
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
    
    public class ExpressionCalculator : IExpressionVisitor
    {
        public double Result;
        
        public void Visit(DoubleExpression de)
        {
            Result = de.Value;
        }

        public void Visit(AdditionExpression ae)
        {
            ae.Left.Accept(this);
            var a = Result;
            ae.Right.Accept(this);
            var b = Result;
            Result = a + b;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var e = new AdditionExpression(
                new DoubleExpression(1),
                new AdditionExpression(
                    new DoubleExpression(2),
                    new DoubleExpression(3)
                    ));
            
            var ep = new ExpressionPrinter();
            ep.Visit(e);
            Console.WriteLine(ep);
            
            var calc = new ExpressionCalculator();
            calc.Visit(e);
            Console.WriteLine($"{ep} = {calc.Result}");
        }
    }
}