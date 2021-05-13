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
using System.Collections.Generic;
using jt.jestpro.Helpers;
using Microsoft.Extensions.Options;

namespace jt.jestpro.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        ILogger<ReportController> _logger;
        IReportService _service;
        private readonly AppSettings _appSettings;

        public ReportController(ILogger<ReportController> logger, IReportService currentService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _service = currentService;
            _appSettings = appSettings.Value;
        }

        // GET api/Report/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<ReportDto[]>> GetList([FromQuery] ReportFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Report/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<ReportDto[]>> GetListFiltered([FromBody] ReportFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Report/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/Report
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ReportDto>> Post([FromBody] ReportEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/Report/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<ActionResult<ReportDto>> Put([FromBody] ReportEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a Report without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/Report/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("GetLog")]
        public IActionResult GetLog([FromQuery] string filename)
        {
            var filePath = System.IO.Path.Combine(_appSettings.LogPath, filename);
            return PhysicalFile(filePath, MimeTypes.GetMimeType(filePath), filePath);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("GetLastLogs")]
        public async Task<ActionResult<List<string>>> GetLastLog()
        {
            var res = await _service.GetLastLogs();
            return res;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser, Watcher")]
        [HttpPost("GetReportCashDesk")]
        public async Task<ActionResult<PaymentReceiptDto[]>> GetReportCashDesk([FromBody] ReportCashDeskFilter filter)
        {
            var res = await _service.GetReportCashDesk(filter);
            return res;
        }

        [HttpPost("ExportInExcel")]
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [ProducesResponseType(typeof(File), 200)]
        public async Task<IActionResult> ExportInExcel([FromBody] ReportExcelDto parameters)
        {

            var content = await _service.ExportInExcel(parameters);

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"export_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx");
        }


    }
}
