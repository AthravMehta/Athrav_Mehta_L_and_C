using Assignment_5.Question1;
using Assignment_5.Question2;

namespace Assignment_5
{
    public class Assignment5
    {
        public static void main(string[] args)
        {
            // Answer 1
            // Here employee is the object, it's declaration of an object and Employee is Class of the Object.
            Employee employee = new Employee();

            // Answer 2
            Wallet wallet = new Wallet(10.00f);
            Customer customer = new Customer("John", "Doe", wallet);
            DeliveryBoy deliveryBoy = new DeliveryBoy();
            deliveryBoy.GetPayment(customer);

            Console.WriteLine($"Customer's remaining balance: {customer.GetTotalMoney()}");
        }
    }
}
