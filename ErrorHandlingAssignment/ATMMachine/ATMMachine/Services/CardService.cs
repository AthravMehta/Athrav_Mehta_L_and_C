using ATMMachine.Exceptions;
using ATMMachine.Interfaces;
using ATMMachine.Models;

namespace ATMMachine.Services
{
    public class CardService : ICardService
    {
        private readonly List<CardModel> _cards = new();
        private int _cardIdCounter = 1;

        public CardModel LinkCardToAccount(int accountId, int pin)
        {

            var card = new CardModel
            {
                Id = _cardIdCounter++,
                AccountId = accountId,
                Pin = pin
            };

            _cards.Add(card);
            Console.WriteLine("Card linked successfully.");
            return card;
        }

        public CardModel? GetCardForAccount(AccountModel account)
        {
            return _cards.FirstOrDefault(c => c.AccountId == account.Id);
        }

        public bool ValidatePin(CardModel card, int enteredPin)
        {
            if (card.IsBlocked)
            {
                throw new CardBlockedException();
            }

            if (card.Pin == enteredPin)
            {
                card.FailedAttempts = 0;
                return true;
            }
            else
            {
                card.FailedAttempts++;
                if (card.IsBlocked)
                {
                    throw new CardBlockedException();
                }
                Console.WriteLine("Invalid PIN. Attempt {0}/3", card.FailedAttempts);
                return false;
            }
        }

    }
}
