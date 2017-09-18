using System;

namespace Contracts
{
    public interface IServerContract
    {
        void Connect(string name);

        Tuple<string, string> CreateNewGame();

        void DoStep(string word);
    }
}