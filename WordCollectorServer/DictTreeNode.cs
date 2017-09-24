using System.Collections.Generic;

namespace WordCollectorServer
{
    class DictTreeNode
    {
        public char Symbol { get; }

        int minWordLength { get; set; }

        int avgWordLength { get; set; }

        int maxWordLength { get; set; }

        SortedDictionary<char, DictTreeNode> Childs = new SortedDictionary<char, DictTreeNode>();

        public DictTreeNode(char symbol)
        {
            this.Symbol = symbol;
            this.minWordLength = int.MaxValue;
            this.maxWordLength = int.MinValue;
        }

        public DictTreeNode AddChild(char symbol)
        {
            DictTreeNode childNode;
            if (this.Childs.TryGetValue(symbol, out childNode))
                return childNode;

            childNode = new DictTreeNode(symbol);
            this.Childs.Add(symbol, childNode);
            return childNode;
        }

        public void RefreshWordLength(int currLength)
        {
            if (currLength < this.minWordLength)
                this.minWordLength = currLength;
            else if (currLength > this.maxWordLength)
                this.maxWordLength = currLength;
            else
                return;

            this.avgWordLength = 
                (this.maxWordLength + this.minWordLength) / 2;
        }

        public DictTreeNode GetChild(char ch)
        {
            DictTreeNode child;
            return this.Childs.TryGetValue(ch, out child) ? child : null;
        }
    }
}

