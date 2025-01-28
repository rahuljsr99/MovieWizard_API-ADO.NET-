using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;
using System.Transactions;


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

        public async Task<int> ProcessInvoiceToDb(Invoice invoice)
        {

            var directorId = await _directorService.GetDirectorIdByName(invoice.DirectorName);
            var movieId = await _movieService.GetMovieIdByName(invoice.MovieName);
            var transaction = await GetTransaction(invoice.InvoiceNumber);

            Invoice DbInvoicePayload = new()
            {
                DirectorId = directorId,
                MovieId = movieId,
                TransactionId = transaction.TransactionId,
                Amount = transaction.Amount,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "User",
                ModeOfPayment = "UPI",
            };

            var result = _transactionalRepository.SaveInvoiceToDb(DbInvoicePayload);

            return result;
        }

        public async Task<Transactions> GetTransaction(int invoiceNumber)
        {
            var transaction = await _transactionalRepository.GetTransactionAsync(invoiceNumber);
            return transaction;
        }

        public async Task SaveTransaction(Transaction transaction)
        {
            
        }
    }
}
