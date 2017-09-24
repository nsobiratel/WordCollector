using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Text;

namespace WordCollectorServer
{
    class Game
    {
        static List<string> words = new List<string>();
        static DictTree dictionary = new DictTree();

        public string Id { get; }

        User User1 { get; }

        User User2 { get; }

        string Word { get; }

        public DictTreeNode currentChar { get; private set; }

        static Game()
        {
            words = File.ReadAllLines("zdf-win.txt", Encoding.GetEncoding(1251))
                .Select(w => w.ToUpperInvariant())
                .Where(w => w.Length > 3 && w.All(ch => char.IsLetter(ch)))
                .ToList();

            foreach (string word in words)
            {
                dictionary.AddWord(word);
            }
        }

        public Game(User user1, User user2, int rndChar)
        {
            this.Id = Guid.NewGuid().ToString();
            this.User1 = user1;
            this.User1.CurrentGameId = this.Id;
            this.User2 = user2;
            this.User2.CurrentGameId = this.Id;

            this.currentChar = dictionary.GetNode(rndChar);
        }

        public User GetEnemyForUser(User user)
        {
            return this.User1 == user ? this.User2 : this.User1;
        }

        public void Finish()
        {
            this.User1.CurrentGameId = string.Empty;
            this.User2.CurrentGameId = string.Empty;
        }

        public bool ValidateStep(char lastChar)
        {
            DictTreeNode newCharNode = this.currentChar.GetChild(lastChar);
            if (newCharNode == null)
                return false;

            this.currentChar = newCharNode;
            return true;
        }
    }
}