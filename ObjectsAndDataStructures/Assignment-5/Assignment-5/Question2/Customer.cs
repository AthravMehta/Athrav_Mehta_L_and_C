namespace Assignment_5.Question2
{
    public class Customer
    {
        private string firstName;
        private string lastName;
        private Wallet myWallet;

        public Customer(string firstName, string lastName, Wallet wallet)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            myWallet = wallet;
        }

        public string FirstName
        {
            get { return firstName; }
        }

        public string LastName
        {
            get { return lastName; }
        }

        public Wallet Wallet
        {
            get { return myWallet; }
        }

        // Added Methods in Customer Class to interact with the wallet to encapsulate better
        public float GetTotalMoney()
        {
            return myWallet.GetTotalMoney();
        }

        public void DepositMoney(float deposit)
        {
            myWallet.AddMoney(deposit);
        }

        public bool WithdrawMoney(float debit)
        {
            if (myWallet.GetTotalMoney() >= debit)
            {
                myWallet.DebitMoney(debit);
                return true;
            }
            return false;
        }
    }

}
