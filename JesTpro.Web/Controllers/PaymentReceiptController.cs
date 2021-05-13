// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.Linq;
using jt.jestpro.Services;
using jt.jestpro.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using jt.jestpro.Helpers;

namespace jt.jestpro.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReceiptController : ControllerBase
    {
        ILogger<PaymentReceiptController> _logger;
        IPaymentReceiptService _service;

        public PaymentReceiptController(ILogger<PaymentReceiptController> logger, IPaymentReceiptService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/PaymentReceipt/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<PaymentReceiptDto[]>> GetList([FromQuery] PaymentReceiptFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/PaymentReceipt/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<PaymentReceiptDto[]>> GetListFiltered([FromBody] PaymentReceiptFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/PaymentReceipt/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentReceiptDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/PaymentReceipt
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<PaymentReceiptDto>> Post([FromBody] PaymentReceiptEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/PaymentReceipt/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<PaymentReceiptDto>> Put([FromBody] PaymentReceiptEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a PaymentReceipt without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/PaymentReceipt/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }


        // POST api/PaymentReceipt
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost("GenerateInvoice")]
        public async Task<ActionResult<PaymentReceiptDto>> GenerateInvoice([FromBody] InvoiceRequestData data)
        {
            var res = await _service.GenerateInvoice(data);
            return res;
        }
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost("CreateFastInvoice")]
        public async Task<ActionResult<PaymentReceiptDto>> CreateFastInvoice([FromBody] PaymentReceiptEditForFastInvoceDto data)
        {
            var res = await _service.CreateFastInvoice(data);
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("generatePdfInvoice/{invoiceId}")]
        public async Task<ActionResult<bool>> GeneratePdfInvoice(Guid invoiceId)
        {
            var res = await _service.GeneratePdfInvoice(invoiceId);
            return res;
        }
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("download/{invoiceId}")]
        public async Task<IActionResult> Download(Guid invoiceId)
        {
            var invoicePath = await _service.GetPdfInvoicePath(invoiceId);
            return PhysicalFile(invoicePath, MimeTypes.GetMimeType(invoicePath), Path.GetFileName(invoicePath));
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("sendInvoice/{invoiceId}")]
        public async Task<ActionResult<string>> SendInvoice(Guid invoiceId)
        {
            var res = await _service.SendPdfInvoiceViaMail(invoiceId);
            return res;
        }

    }
}
