namespace ExpenseTracker.UI.Models
{
    public record ExpenseDetailsDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.PLN;
        public decimal Rate { get; set; } = decimal.One;
        public CategoryDTO Category { get; set; } = null!;
        public decimal TotalAmount => GetTotalAmount();

        private decimal GetTotalAmount()
        {
            if (Rate <= 0)
            {
                return Amount;
            }

            return Amount / Rate;
        }
    }
}
