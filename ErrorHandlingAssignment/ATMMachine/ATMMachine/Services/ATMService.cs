using ATMMachine.Exceptions;
using ATMMachine.Interfaces;
using ATMMachine.Models;

namespace ATMMachine.Services
{
    public class ATMService : IATMService
    {
        private readonly ATMModel _atm;

        public ATMService( ATMModel atm)
        {
            _atm = atm;
        }

        public bool ConnectToServer()
        {
            return new Random().Next(0, 2) == 1;
        }

        public void Withdraw(AccountModel account, decimal amount, CardModel card) 
        {
            if(card.IsBlocked)
                throw new CardBlockedException();
            
            if (amount > account.DailyLimit - account.DailyUsed)
                throw new DailyLimitExceededException();

            if (amount > account.Balance)
                throw new InsufficientFundsException();

            if (amount > _atm.BalanceCapacity - _atm.DailyUsed)
                throw new AtmRanOutOfCashException();

            account.Balance -= amount;
            account.DailyUsed += amount;
            _atm.DailyUsed += amount;

            Console.WriteLine($"Withdrawn: {amount}. Remaining Balance: {account.Balance}");
        }

        public void Deposit(AccountModel account, decimal amount, CardModel card)
        {
            if (card.IsBlocked)
                throw new CardBlockedException();

            account.Balance += amount;
            _atm.BalanceCapacity += amount;
            Console.WriteLine($"Deposited: {amount}. New Balance: {account.Balance}");
        }

        public void CheckBalance(AccountModel account)
        {
            Console.WriteLine($"Your current balance is: {account.Balance}");
        }
    }
}
