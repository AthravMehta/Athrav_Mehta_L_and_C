namespace Assignment_5.Question2
{
    public class Wallet
    {
        private float value;

        public Wallet(float initialValue)
        {
            value = initialValue;
        }

        public float GetTotalMoney()
        {
            return value;
        }

        public void AddMoney(float deposit)
        {
            value += deposit;
        }

        public void DebitMoney(float debit)
        {
            value -= debit;
        }
    }

}
