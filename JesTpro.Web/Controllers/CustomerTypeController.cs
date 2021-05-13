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
    public class CustomerTypeController : ControllerBase
    {
        ILogger<CustomerTypeController> _logger;
        ICustomerTypeService _service;

        public CustomerTypeController(ILogger<CustomerTypeController> logger, ICustomerTypeService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/CustomerType/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<CustomerTypeDto[]>> GetList([FromQuery] CustomerTypeFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/CustomerType/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<CustomerTypeDto[]>> GetListFiltered([FromBody] CustomerTypeFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/CustomerType/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerTypeDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/CustomerType
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPost]
        public async Task<ActionResult<CustomerTypeDto>> Post([FromBody] CustomerTypeEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/CustomerType/5
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPut]
        public async Task<ActionResult<CustomerTypeDto>> Put([FromBody] CustomerTypeEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a CustomerType without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/CustomerType/5
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
