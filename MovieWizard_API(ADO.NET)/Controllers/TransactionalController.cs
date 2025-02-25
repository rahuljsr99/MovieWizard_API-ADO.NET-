using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionalController : Controller
    {
        private readonly ITransactionalService _transactionService;

        public TransactionalController(ITransactionalService transactionalSevice)
        {
            _transactionService = transactionalSevice;
        }

        // Endpoint to get the latest invoice number
        [HttpGet("GetLatestInvoiceNumber")]
        public IActionResult GetLatestInvoiceNumber()
        {
            var invoiceNumber = _transactionService.GetLatestInvoiceNumber();

            if (invoiceNumber == null)
            {
                return NotFound("Invoice number not found");
            }
            else
            {
                return Ok(invoiceNumber);
            }
        }

        // Endpoint to save invoice and return the invoice number
        [HttpPost("SaveInvoice")]
        public async Task<ActionResult<string>> SaveInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invalid Input");
            }
            else
            {
                var invoiceNumber = await _transactionService.ProcessInvoiceToDb(invoice);
                var response = new
                {
                    status = 200,
                    invoiceNumber = invoiceNumber
                };
                return Ok(response);
            }
        }

        // Test endpoint
        [HttpGet("GetInvoice")]
        public async Task<ActionResult<Invoice>> GetInvoice(int invoiceNumber)
        {
            Invoice invoice = await _transactionService.GetInvoice(invoiceNumber);
            if (invoice == null)
            {
                return NotFound("Invoice not found!");
            }
            else
            {
                return Ok(invoice);
            }
        }

        [HttpGet("GetTotalRevenue")]

        public async Task<double> GetTotalRevenue()
        {
            var totalRevenue = _transactionService.GetTotalRevenue().Result;
            if (totalRevenue == null)
            {
                return 0;
            }
            else
            {
                return totalRevenue;
            }
        }
    }
}
