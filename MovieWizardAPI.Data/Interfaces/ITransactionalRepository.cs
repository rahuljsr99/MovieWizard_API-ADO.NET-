using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MovieWizardAPI.Models;
using MovieWizardAPI.Models.ResponseModels;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface ITransactionalRepository
    {
        Task<int> GetLatestInvoiceNumber();
	    Task<Transactions> GetTransactionAsync(int invoiceNumber);

        Task<int> SaveInvoiceToDb(Invoice invoice);

        Task<Invoice?> GetInvoiceByInvoiceNumber(int invoiceNumber);

        Task<TotalRevenue> GetTotalRevenue();

    }
}
