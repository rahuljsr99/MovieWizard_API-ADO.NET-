using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class Invoice
    {
        public int InvoiceNumber { get; set; }
        public int? MovieId { get; set; }
        public string? MovieName { get; set; }
        public int? DirectorId { get; set; }
        public string? DirectorName { get; set; }
        public string? TransactionId { get; set; }
        public string? ModeOfPayment { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public double? Amount { get; set; }
    }
}
