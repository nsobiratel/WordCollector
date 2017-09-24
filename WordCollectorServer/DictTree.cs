using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCollectorServer
{
    public class DictTree
    {
        SortedDictionary<char, DictTreeNode> Nodes = new SortedDictionary<char, DictTreeNode>();

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

        internal DictTreeNode GetNode(int rndChar)
        {
            return this.Nodes.Values.ElementAt(rndChar);
        }
    }
}

