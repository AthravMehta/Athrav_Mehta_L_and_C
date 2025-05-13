using ATMMachine.Interfaces;
using ATMMachine.Models;

namespace ATMMachine.Services
{
    public class AccountService : IAccountService
    {
        private readonly List<AccountModel> _accounts = new();
        private int _accountIdCounter = 1;

        public AccountModel CreateAccount(string name,  string email, string password)
        {
            if (_accounts.Any(a => a.Email == email))
                throw new Exception("Account with this email already exists.");

            var account = new AccountModel
            {
                Id = _accountIdCounter++,
                Name = name,
                Email = email,
                Password = password
            };

            _accounts.Add(account);

            return account;
        }

        public AccountModel Login(string email, string password)
        {
            var account = _accounts.FirstOrDefault(a => a.Email == email && a.Password == password);
            if (account == null)
                throw new Exception("Invalid email or password.");
            return account;
        }

        public AccountModel GetAccountByEmail(string email)
        {
            return _accounts.FirstOrDefault(a => a.Email == email);
        }

    }
}
