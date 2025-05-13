namespace ATMMachine.Models
{
    public class ATMModel
    {
        public int AtmId { get; set; }
        public string location { get; set; }
        public decimal BalanceCapacity { get; set; } = 100000;
        public decimal DailyUsed { get; set; } = 0;

    }
}
