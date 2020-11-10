using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using DesignPatterns1.CreationalPatterns.Builder;

namespace DesignPatterns1.StructuralPatterns.Mediator
{
    public class PersonM
    {
        public string Name;
        public ChatRoom room;
        private List<string> chatLog = new List<string>();

        public PersonM(string name)
        {
            Name = name;
        }

        public void Say(string message)
        {
            room.Broadcast(Name, message);
        }

        public void PrivateMessage(string who, string message)
        {
            room.Message(Name, who, message);
        }

        public void Receive(string sender, string message)
        {
            string s = $"{sender}: '{message}'";
            chatLog.Add(s);
            Console.WriteLine($"[{Name}'s chat session] {s}");
        }
    }

    public class ChatRoom
    {
        private List<PersonM> people = new List<PersonM>();
        
        public void Join(PersonM p)
        {
            string joinMsg = $"{p.Name} joins the chat";
            Broadcast("Room", joinMsg);

            p.room = this;
            people.Add(p);
        }

        public void Broadcast(string source, string message)
        {
            foreach (var p in people)
                if (p.Name != source)
                    p.Receive(source, message);
        }

        public void Message(string source, string destination, string message)
        {
            people.FirstOrDefault(p => p.Name == destination)
                ?.Receive(source, message);
        }
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var room = new ChatRoom();
    //         
    //         var john = new PersonM("John");
    //         var jane = new PersonM("Jane");
    //
    //         room.Join(john);
    //         room.Join(jane);
    //
    //         john.Say("hi");
    //         jane.Say("oh, hey john");
    //         
    //         var simon = new PersonM("Simon");
    //         room.Join(simon);
    //         simon.Say("hi everyone");
    //         
    //         jane.PrivateMessage("Simon", "Glad you could join us");
    //     }
    // }
}