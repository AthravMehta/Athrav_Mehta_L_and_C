using ATMMachine.Models;

namespace ATMMachine.Interfaces
{
    public interface ICardService
    {
        CardModel LinkCardToAccount(int accountId, int pin);
        CardModel? GetCardForAccount(AccountModel account);
        bool ValidatePin(CardModel card, int enteredPin);
    }
}
