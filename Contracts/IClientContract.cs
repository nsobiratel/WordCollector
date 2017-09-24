namespace Contracts
{
    public interface IClientContract
    {
        void OnCanDoStep(char lastChar);

        void OnShowMessage(string msg);

        void OnGameStarted(string gameId, string enemyNick, char startChar);

        void OnGameFinished(string winner, string reason);
    }
}

