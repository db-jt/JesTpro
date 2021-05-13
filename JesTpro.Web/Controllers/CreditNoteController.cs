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
    public class CreditNoteController : ControllerBase
    {
        ILogger<CreditNoteController> _logger;
        ICreditNoteService _service;

        public CreditNoteController(ILogger<CreditNoteController> logger, ICreditNoteService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/CreditNote/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<CreditNoteDto[]>> GetList([FromQuery] CreditNoteFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/CreditNote/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<CreditNoteDto[]>> GetListFiltered([FromBody] CreditNoteFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/CreditNote/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CreditNoteDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/CreditNote
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<CreditNoteDto>> Post([FromBody] CreditNoteEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/CreditNote/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("download/{creditNoteId}")]
        public async Task<IActionResult> Download(Guid creditNoteId)
        {
            var creditNotePath = await _service.GetPdfCreditNotePath(creditNoteId);
            return PhysicalFile(creditNotePath, MimeTypes.GetMimeType(creditNotePath), Path.GetFileName(creditNotePath));
        }

    }
}
