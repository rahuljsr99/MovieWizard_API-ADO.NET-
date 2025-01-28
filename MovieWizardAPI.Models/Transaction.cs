namespace MovieWizardAPI.Models
{
    public class Transactions
    {
        public string? TransactionId {  get; set; }
        public double? Amount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? InvoiceNumber { get; set; }
        public string? CreatedBy { get; set; }
    }
}
