using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Configuration;

namespace WordCollectorServer
{
    class Game
    {
        static List<string> words = new List<string>();

        public string Id { get; }

        User User1 { get; }

        User User2 { get; }

        string Word { get; }

        static Game()
        {
            words = File.ReadAllLines("word_rus.txt")
                .Select(w => w.ToUpperInvariant())
                .ToList();
        }

        public Game(User user1, User user2)
        {
            this.Id = Guid.NewGuid().ToString();
            this.User1 = user1;
            this.User2 = user2;
        }
    }
}