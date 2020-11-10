using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DesignPatterns1.CreationalPatterns.Builder
{
    public class LifeWithoutBuilder
    {
        public static void ExampleWithoutBuilder()
        {
            var hello = "hello";
            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(hello);
            sb.Append("</p>");

            Console.WriteLine(sb);

            var words = new[] {"hello", "world"};
            sb.Clear();
            sb.Append("<ul>");
            foreach (var word in words)
            {
                sb.AppendFormat($"<li>{word}</li>");
            }

            Console.WriteLine(sb);
        }

        public static void ExampleWithBuilder()
        {
            var builder = new HtmlBuilder("ul");
            builder.AddChild("li", "hello").AddChild("li", "world");

            Console.WriteLine(builder);
        }
    }

    public class HtmlElement
    {
        public string Name, Text;
        public List<HtmlElement> Elements = new List<HtmlElement>();
        private const int indentSize = 2;

        public HtmlElement()
        {
            
        }

        public HtmlElement(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indentSize * indent);
            sb.Append($"{i}<{Name}>\n");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', indentSize * (indent + 1)));
                sb.AppendLine(Text);
            }

            foreach (var e in Elements)
            {
                sb.Append(e.ToStringImpl(indent + 1));
            }
            
            sb.Append($"{i}</{Name}>\n");
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    public class HtmlBuilder
    {
        private readonly string _rootName;
        HtmlElement root = new HtmlElement();

        public HtmlBuilder(string rootName)
        {
            _rootName = rootName;
            root.Name = rootName;
        }

        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new HtmlElement {Name = _rootName};
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         LifeWithoutBuilder.ExampleWithoutBuilder();
    //         
    //         LifeWithoutBuilder.ExampleWithBuilder();
    //     }
    // }
}