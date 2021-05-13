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

namespace jt.jestpro.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReceiptDetailController : ControllerBase
    {
        ILogger<PaymentReceiptDetailController> _logger;
        IPaymentReceiptDetailService _service;

        public PaymentReceiptDetailController(ILogger<PaymentReceiptDetailController> logger, IPaymentReceiptDetailService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/PaymentReceiptDetail/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<PaymentReceiptDetailDto[]>> GetList([FromQuery] PaymentReceiptDetailFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/PaymentReceiptDetail/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<PaymentReceiptDetailDto[]>> GetListFiltered([FromBody] PaymentReceiptDetailFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/PaymentReceiptDetail/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentReceiptDetailDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/PaymentReceiptDetail
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<PaymentReceiptDetailDto>> Post([FromBody] PaymentReceiptDetailEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/PaymentReceiptDetail/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<PaymentReceiptDetailDto>> Put([FromBody] PaymentReceiptDetailEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a PaymentReceiptDetail without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/PaymentReceiptDetail/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
