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

        [HttpGet("GetLatestInvoiceNumber")]
        public IActionResult GetLatestInvoiceNumber()
        {

            var invoiceNumber = _transactionService.GetLatestInvoiceNumber();

            if (invoiceNumber == null)
            {
                return NotFound($"Null");
            }
            else
            {
                return Ok(invoiceNumber);
            }
        }


        [HttpPost("SaveInvoice")]
        public async Task<ActionResult<int>> SaveInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invalid Input");
            }
            else
            {
                var invoiceNumber = await _transactionService.ProcessInvoiceToDb(invoice);
                return Ok($"Invoice Saved, Invoice Number : {invoiceNumber}"); 
            }
        }
    }

}
