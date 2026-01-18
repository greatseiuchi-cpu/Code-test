
namespace CsvLedger
{
    public class Transaction
    {
        public DateOnly Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
