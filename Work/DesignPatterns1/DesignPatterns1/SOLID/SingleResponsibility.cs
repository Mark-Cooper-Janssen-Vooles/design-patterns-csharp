using System;
using System.Collections.Generic;
using System.IO;

namespace DesignPatterns1.SOLID
{
    // SingleResponsibility
    public class Journal
    {
        private readonly List<string> entries = new List<string>();
        private static int _count = 0;

        public int AddEntry(string text)
        {
            entries.Add($"{++_count}: {text}");
            return _count; //memento pattern
        }

        public void RemoveEntry(int index)
        {
            entries.RemoveAt(index);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }
    }

    public class Persistence
    {
        public void SaveToFile(Journal j, string filename, bool overwrite = false)
        {
            if (overwrite || !File.Exists(filename))
                File.WriteAllText(filename, j.ToString());
        }
    }
}