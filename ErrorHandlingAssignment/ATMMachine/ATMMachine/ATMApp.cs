using ATMMachine.Constants;
using ATMMachine.Exceptions;
using ATMMachine.Models;
using ATMMachine.Services;
using ATMMachine.Utils;

namespace ATMMachine
{
    public class ATMApp
    {
        private readonly AccountService _accountService = new();
        private readonly CardService _cardService = new();
        private readonly ATMService _atmService;

        private AccountModel? _currentAccount;
        private CardModel? _currentCard;

        public ATMApp()
        {
            var atm = new ATMModel { AtmId = 1, location = "Kanakpura" };
            _atmService = new ATMService(atm);
        }

        public void Run()
        {
            try
            {
                while (true)
                {
                    ShowMainMenu();
                    var choice = InputHelper.ReadNonEmptyString("Choose an option: ");
                    if (!checkConnectionToServer())
                    {
                        throw new UnableToConnectToServerException();
                    }
                    switch (choice)
                    {
                        case "1":
                            if (!HandleAccountCreation()) continue;
                            break;
                        case "2":
                            if (!HandleLogin()) continue;
                            break;
                        case "3":
                            return;
                        default:
                            Console.WriteLine("Invalid option.\n");
                            continue;
                    }

                    RunATMOperations();
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);    
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("\nATM Machine:");
            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit\n");
        }
        private void ShowAtmMenu()
        {
            Console.WriteLine("\nATM Operations:");
            Console.WriteLine("1. Withdraw");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Check Balance");
            Console.WriteLine("4. Logout");
        }

        private void RunATMOperations()
        {
            while (_currentAccount != null && _currentCard != null)
            {
                ShowAtmMenu();
                string op = InputHelper.ReadNonEmptyString("Choose an operation: ");

                if (!checkConnectionToServer())
                {
                    throw new UnableToConnectToServerException();
                }

                try
                {
                    switch (op)
                    {
                        case "1": HandleWithdraw(); break;
                        case "2": HandleDeposit(); break;
                        case "3": HandleCheckBalance(); break;
                        case "4": HandleLogout(); break;
                        default: Console.WriteLine("Invalid option."); break;
                    }

                    if (op == "4") break;
                }
                catch (ATMException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        private bool HandleAccountCreation()
        {
            try
            {
                string name = InputHelper.ReadNonEmptyString("Enter Name: ");
                string email = InputHelper.ReadEmail("Enter Email: ");
                string password = InputHelper.ReadPassword("Enter Password: ");
                int pin = InputHelper.ReadPin("Set 4-digit PIN: ");

                _currentAccount = _accountService.CreateAccount(name, email, password);
                _currentCard = _cardService.LinkCardToAccount(_currentAccount.Id, pin);
                Console.WriteLine($"Account Created! Account ID: {_currentAccount.Id}\n");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Account Creation Failed: {ex.Message}");
                ClearSession();
                return false;
            }
        }

        private bool HandleLogin()
        {
            try
            {
                string email = InputHelper.ReadEmail("Enter Email: ");
                string password = InputHelper.ReadPassword("Enter Password: ");

                _currentAccount = _accountService.Login(email, password);
                _currentCard = _cardService.GetCardForAccount(_currentAccount);
                Console.WriteLine($"Welcome back, {_currentAccount.Name}!\n");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                ClearSession();
                return false;
            }
        }

        private void HandleWithdraw()
        {
            ValidateSession();
            ValidateCard();

            int pin = InputHelper.ReadPin("Enter your PIN: ");
            if (!_cardService.ValidatePin(_currentCard!, pin)) return;

            decimal amount = InputHelper.ReadDecimal("Enter amount to withdraw: ");
            if (amount <= 0) throw new ATMException(ErrorConstant.AmountGreaterThanZero);

            _atmService.Withdraw(_currentAccount!, amount, _currentCard!);
        }

        private void HandleDeposit()
        {
            ValidateSession();
            ValidateCard();

            int pin = InputHelper.ReadPin("Enter your PIN: ");
            if (!_cardService.ValidatePin(_currentCard!, pin)) return;

            decimal amount = InputHelper.ReadDecimal("Enter amount to deposit: ");
            if (amount <= 0) throw new ATMException(ErrorConstant.AmountGreaterThanZero);

            _atmService.Deposit(_currentAccount!, amount, _currentCard!);
        }

        private void HandleCheckBalance()
        {
            ValidateSession();
            ValidateCard();

            int pin = InputHelper.ReadPin("Enter your PIN: ");
            if (!_cardService.ValidatePin(_currentCard!, pin)) return;

            _atmService.CheckBalance(_currentAccount!);
        }

        private void HandleLogout()
        {
            Console.WriteLine("Logged out.");
            ClearSession();
        }

        private bool checkConnectionToServer()
        {
            return _atmService.ConnectToServer();
        }

        private void ValidateSession()
        {
            if (_currentAccount == null || _currentCard == null)
                throw new AccountAndCardException();
        }

        private void ValidateCard()
        {
            if (_currentCard!.IsBlocked)
                throw new CardBlockedException();
        }

        private void ClearSession()
        {
            _currentAccount = null;
            _currentCard = null;
        }
    }
}
