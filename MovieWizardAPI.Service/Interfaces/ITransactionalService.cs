using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface ITransactionalService
    {
        int GetLatestInvoiceNumber();
        Task<int> ProcessInvoiceToDb(Invoice invoice);
        Task<Transactions> GetTransaction(int invoiceNumber);
    }
}
