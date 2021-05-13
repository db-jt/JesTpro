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
    public class ProductSessionAttendanceController : ControllerBase
    {
        ILogger<ProductSessionAttendanceController> _logger;
        IProductSessionAttendanceService _service;

        public ProductSessionAttendanceController(ILogger<ProductSessionAttendanceController> logger, IProductSessionAttendanceService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/ProductSessionAttendance/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<ProductSessionAttendanceDto[]>> GetList([FromQuery] ProductSessionAttendanceFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/ProductSessionAttendance/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<ProductSessionAttendanceDto[]>> GetListFiltered([FromBody] ProductSessionAttendanceFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/ProductSessionAttendance/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSessionAttendanceDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/ProductSessionAttendance
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpPost]
        public async Task<ActionResult<ProductSessionAttendanceDto>> Post([FromBody] ProductSessionAttendanceEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/ProductSessionAttendance/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpPut]
        public async Task<ActionResult<ProductSessionAttendanceDto>> Put([FromBody] ProductSessionAttendanceEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a ProductSessionAttendance without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/ProductSessionAttendance/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
