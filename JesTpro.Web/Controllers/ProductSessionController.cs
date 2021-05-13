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
    public class ProductSessionController : ControllerBase
    {
        ILogger<ProductSessionController> _logger;
        IProductSessionService _service;

        public ProductSessionController(ILogger<ProductSessionController> logger, IProductSessionService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/ProductSession/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<ProductSessionDto[]>> GetList([FromQuery] ProductSessionFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/ProductSession/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<ProductSessionDto[]>> GetListFiltered([FromBody] ProductSessionFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/ProductSession/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSessionDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/ProductSession
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpPost]
        public async Task<ActionResult<ProductSessionDto>> Post([FromBody] ProductSessionEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpPost("SaveSubscribers")]
        public async Task<ActionResult<ProductSessionDto>> SaveSubscribers([FromBody] ProductSessionDto value)
        {
            var res = await _service.SaveSubscribers(value);
            return res;
        }

        // PUT api/ProductSession/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpPut]
        public async Task<ActionResult<ProductSessionDto>> Put([FromBody] ProductSessionEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a ProductSession without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/ProductSession/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Teacher")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
