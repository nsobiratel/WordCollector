using System;

namespace Contracts
{
    public interface IServerContract
    {
        void Connect(string name);

        Tuple<string, string, char> CreateNewGame();

        void DoStep(string gameId, char lastSymbol);
    }
}