using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns1.CreationalPatterns.Builder
{

    public class CodeBuilder
    {
        private string ClassName;
        private List<string[]> Fields = new List<string[]>();

        public CodeBuilder(string className)
        {
            ClassName = className;
        }

        public CodeBuilder AddField(string name, string type)
        {
            var entry = new string[] {name, type};
            Fields.Add(entry);
            return this;
        }

        public override string ToString()
        {
            var formattedFields = FormatFields();

            return $"public class {ClassName} \n" +
                   "{\n" + 
                   $"{formattedFields}" +
                    "}";
        }

        private string FormatFields()
        {
            var sb = new StringBuilder();
            
            foreach (var field in Fields)
            {
                sb.Append($"  public {field[0]} {field[1]} \n");
            }

            return sb.ToString();
        }
    }
    
    class Program
    {
        // static void Main(string[] args)
        // {
        //     var cb = new CodeBuilder("Person").AddField("Name", "String").AddField("Age", "int");
        //     Console.WriteLine(cb);
        // }
    }
}