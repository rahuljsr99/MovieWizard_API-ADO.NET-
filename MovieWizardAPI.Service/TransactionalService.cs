using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;
using System.Transactions;
using System.Text;
using System.Globalization;
using MovieWizardAPI.Models.ResponseModels;


namespace MovieWizardAPI.Service
{
    public class TransactionalService : ITransactionalService
    {
        private readonly ITransactionalRepository _transactionalRepository;
        private readonly IDirectorService _directorService;
        private readonly IMovieService _movieService;

        public TransactionalService(ITransactionalRepository transactionalRepository, IDirectorService directorService, IMovieService movieService) {
            
            _transactionalRepository = transactionalRepository;
            _directorService = directorService;
            _movieService = movieService;
        }

        public async Task<int> GetLatestInvoiceNumber()
        {
            return await _transactionalRepository.GetLatestInvoiceNumber();
        }

        public async Task<int?> ProcessInvoiceToDb(Invoice invoice)
        {

            var directorId = await _directorService.GetDirectorIdByName(invoice.DirectorName);
            var movieId = await _movieService.GetMovieIdByName(invoice.MovieName);
            //var transaction = await GetTransaction(invoice.InvoiceNumber);

            Invoice DbInvoicePayload = new()
            {
                DirectorId = directorId,
                MovieId = movieId,
                TransactionNumber = invoice.TransactionNumber,
                Amount = (double?)invoice.Amount,
                CreatedBy = "User",
                ModeOfPayment = "UPI",
                ItemType = invoice.ItemType
            };

            var result = await _transactionalRepository.SaveInvoiceToDb(DbInvoicePayload);

            return result;
        }

        public async Task<Transactions> GetTransaction(int invoiceNumber)
        {
            var transaction = await _transactionalRepository.GetTransactionAsync(invoiceNumber);
            return transaction;
        }

        public async Task<Invoice?> GetInvoice(int invoiceNumber)
        {
            Invoice? invoice = await _transactionalRepository.GetInvoiceByInvoiceNumber(invoiceNumber);
            if (invoice != null)
            {
                return invoice;
            }
            else
            {
                return null;
            }
        }

        public async Task<TotalRevenue> GetTotalRevenue()
        {
            var totalRevenue = await _transactionalRepository.GetTotalRevenue();

            return (totalRevenue);
        }


    }
}
