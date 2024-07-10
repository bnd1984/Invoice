using InvoiceSystem.Models;
using InvoiceSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InvoiceSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoicesController()
        {
            _invoiceService = new InvoiceService();
        }

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] InvoiceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var createdInvoice = _invoiceService.CreateInvoice(request.Amount, request.DueDate);
            return CreatedAtAction(nameof(GetInvoices), new { id = createdInvoice.Id }, new { id = createdInvoice.Id });
        }

        [HttpGet]
        public IActionResult GetInvoices()
        {
            var invoices = _invoiceService.GetInvoices();
            return Ok(invoices);
        }

        [HttpPost("{id}/payments")]
        public IActionResult PayInvoice(int id, [FromBody] Payment payment)
        {
            try
            {
                _invoiceService.PayInvoice(id, payment.Amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("process-overdue")]
        public IActionResult ProcessOverdueInvoices([FromBody] ProcessOverdueRequest request)
        {
            _invoiceService.ProcessOverdueInvoices(request.LateFee, request.OverdueDays);
            return Ok();
        }
    }
}
