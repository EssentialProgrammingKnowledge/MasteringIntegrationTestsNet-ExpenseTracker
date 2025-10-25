namespace ExpenseTracker.API.DTO
{
    public class NbpTableDTO
    {
        public string Table { get; set; } = null!;
        public string No { get; set; } = null!;
        public DateOnly EffectiveDate { get; set; }
        public List<NbpRateDTO> Rates { get; set; } = [];
    }
}
