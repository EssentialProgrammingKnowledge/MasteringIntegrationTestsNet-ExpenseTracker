namespace ExpenseTracker.API.DTO
{
    public class NbpRateDTO
    {
        public string Currency { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal Mid { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }
}
