using ATMMachine.Models;

namespace ATMMachine.Interfaces
{
    public interface IATMService
    {
        bool ConnectToServer();
        void Withdraw(AccountModel account, decimal amount, CardModel card);
        void Deposit(AccountModel account, decimal amount, CardModel card);
        void CheckBalance (AccountModel account);
    }
}
