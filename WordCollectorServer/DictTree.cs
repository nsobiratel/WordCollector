using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace WordCollectorServer
{
    public class DictTree
    {
        SortedDictionary<char, DictTreeNode> Nodes = new SortedDictionary<char, DictTreeNode>();
        static readonly Random rnd = new Random();

        public DictTree()
        {
            for (char ch = 'А'; ch <= 'Я'; ch++)
            {
                this.Nodes.Add(ch, new DictTreeNode(ch));
            }
            this.Nodes.Add('Ё', new DictTreeNode('Ё'));
        }

        public void AddWord(string word)
        {
            int wordLength = word.Length;
            DictTreeNode node = this.Nodes[word[0]];
            node.RefreshWordLength(wordLength);
            for (int i = 1; i < wordLength; i++)
            {
                node = node.AddChild(word[i]);
                node.RefreshWordLength(wordLength);
            }
        }

        public void RemoveEmptyStartNodes()
        {
            foreach (DictTreeNode node in new List<DictTreeNode>(this.Nodes.Values))
            {
                if (node.HasChild)
                    continue;
                this.Nodes.Remove(node.Symbol);
            }
        }

        internal DictTreeNode GetNode()
        {
            return this.Nodes.Values.ElementAt(rnd.Next(this.Nodes.Count));
        }
    }
}

