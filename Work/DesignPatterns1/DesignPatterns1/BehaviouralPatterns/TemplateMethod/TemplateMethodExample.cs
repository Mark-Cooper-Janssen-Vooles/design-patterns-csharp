using System;
using NUnit.Framework.Constraints;

namespace DesignPatterns1.BehaviouralPatterns.TemplateMethod
{
    //i.e most games have a start, a turn, and output for winner 
    //can make a template method to describe this process
    public abstract class Game
    {
        public void Run()
        {
            Start();
            while (!HaveWinner)
                TakeTurn();
            Console.WriteLine($"Player {WinningPlayer} wins.");
        }

        protected int CurrentPlayer;
        protected readonly int numberOfPlayers;

        protected Game(int numberOfPlayers)
        {
            this.numberOfPlayers = numberOfPlayers;
        }

        protected abstract void Start(); //inheritor needs to fill this in when the game starts
        protected abstract void TakeTurn();
        protected abstract bool HaveWinner { get; }
        protected abstract int WinningPlayer { get; }
    }

    public class Chess : Game
    {
        public Chess() : base(2)
        {
        }

        protected override void Start()
        {
            Console.WriteLine($"Starting a game of chess with {numberOfPlayers} players.");
        }

        protected override void TakeTurn()
        {
            Console.WriteLine($"Turn {turn++} taken by layer {CurrentPlayer}.");
            CurrentPlayer = (CurrentPlayer + 1) % numberOfPlayers;
        }

        protected override bool HaveWinner => turn == maxTurns;
        protected override int WinningPlayer => CurrentPlayer;

        private int turn = 1;
        private int maxTurns = 10;
    }
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var chess = new Chess();
    //         chess.Run(); //it'll call Run in the Game class, but because all the functions in run are abstract, they will call the correct ones in the chess class.
    //     }
    // }
}