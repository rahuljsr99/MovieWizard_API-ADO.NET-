namespace MovieWizardAPI.Models
{
    public class Transactions
    {
        public string? TransactionId {  get; set; }
        public int? Amount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? InvoiceNumber { get; set; }
        public string? CreatedBy { get; set; }
    }
}
