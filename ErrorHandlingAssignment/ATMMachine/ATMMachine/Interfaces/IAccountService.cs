using ATMMachine.Models;

namespace ATMMachine.Interfaces
{
    public interface IAccountService
    {
        AccountModel CreateAccount(string name, string email, string password);
        AccountModel Login(string email, string password);
        AccountModel GetAccountByEmail(string email);
    }
}
