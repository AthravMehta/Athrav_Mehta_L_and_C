namespace ATMMachine.Models
{
    public class CardModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public AccountModel Account { get; set; }
        public int Pin { get; set; }
        public int FailedAttempts { get; set; } = 0;
        public bool IsBlocked => FailedAttempts >= 3;
    }
}
