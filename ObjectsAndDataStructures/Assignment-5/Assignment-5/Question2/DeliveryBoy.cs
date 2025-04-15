namespace Assignment_5.Question2
{
    // Delivery Boy Class is created to Get Payment from Customer
    public class DeliveryBoy
    {
        public void GetPayment(Customer customer)
        {
            float payment = 2.00f;
            if (customer.GetTotalMoney() > payment)
            {
                customer.WithdrawMoney(payment);
                Console.WriteLine("Payment successful.");
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }
    }
}
