namespace ATMMachine.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; } = 0;
        public decimal DailyLimit { get; set; } = 20000;
        public decimal DailyUsed { get; set; } = 0;
        public int CardId { get; set; }
        public CardModel Card { get; set; }
    }
}
