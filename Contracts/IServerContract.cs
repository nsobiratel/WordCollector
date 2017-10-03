using System;

namespace Contracts
{
    public interface IServerContract
    {
        void Connect(string name);

        Tuple<string, string, char> CreateNewGame();

        bool? DoStep(string gameId, char lastSymbol);
    }
}