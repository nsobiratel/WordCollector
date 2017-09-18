using System;

namespace Contracts
{
    public interface IClientContract
    {
        void OnCanDoStep(string word);

        void OnShowMessage(string msg);
    }
}

