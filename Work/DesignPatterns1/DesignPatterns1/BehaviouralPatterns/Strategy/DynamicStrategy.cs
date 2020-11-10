using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns1.BehaviouralPatterns.Strategy
{
    public enum OutputFormat
    {
        Markdown,
        Html
    }

    public interface IListStrategy
    {
        void Start(StringBuilder sb);
        void End(StringBuilder sb);
        void AddListItem(StringBuilder sb, string item);
    }
    
    public class HtmlListStrategy : IListStrategy
    {
        public void Start(StringBuilder sb)
        {
            sb.AppendLine("<ul>");
        }

        public void End(StringBuilder sb)
        {
            sb.AppendLine("</ul>");
        }

        public void AddListItem(StringBuilder sb, string item)
        {
            sb.AppendLine($"  <li>{item}</>");
        }
    }
    
    public class MarkdownListStrategy : IListStrategy
    {
        public void Start(StringBuilder sb)
        {

        }

        public void End(StringBuilder sb)
        {
            
        }

        public void AddListItem(StringBuilder sb, string item)
        {
            sb.AppendLine($"  * {item}");
        }
    }

    public class TextProcessor
    {
        private StringBuilder sb = new StringBuilder();
        private IListStrategy ListStrategy;

        public void SetOutputFormat(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Markdown:
                    ListStrategy = new MarkdownListStrategy();
                    break;
                case OutputFormat.Html:
                    ListStrategy = new HtmlListStrategy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }
        }

        public void AppendList(IEnumerable<string> items)
        {
            ListStrategy.Start(sb);
            foreach (var item in items)
                ListStrategy.AddListItem(sb, item);
            ListStrategy.End(sb);
        }

        public StringBuilder Clear()
        {
            return sb.Clear();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }

    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var tp = new TextProcessor();
    //         tp.SetOutputFormat(OutputFormat.Markdown);
    //         tp.AppendList(new[] { "foo", "bar", "baz"});
    //         Console.WriteLine(tp);
    //
    //         tp.Clear();
    //         
    //         tp.SetOutputFormat(OutputFormat.Html);
    //         tp.AppendList(new[] { "foo", "bar", "baz"});
    //         Console.WriteLine(tp);
    //     }
    // }
}