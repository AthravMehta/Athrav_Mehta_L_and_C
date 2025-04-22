using ATMMachine.Models;

namespace ATMMachine.Interfaces
{
    public interface IATMService
    {
        void Withdraw(AccountModel account, decimal amount, CardModel card);
        void Deposit(AccountModel account, decimal amount, CardModel card);
        void CheckBalance (AccountModel account);
    }
}
