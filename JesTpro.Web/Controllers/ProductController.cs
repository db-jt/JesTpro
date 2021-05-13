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
    public class ProductController : ControllerBase
    {
        ILogger<ProductController> _logger;
        IProductService _service;

        public ProductController(ILogger<ProductController> logger, IProductService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/Product/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<ProductDto[]>> GetList([FromQuery] ProductFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Product/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<ProductDto[]>> GetListFiltered([FromBody] ProductFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Product/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/Product
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Post([FromBody] ProductEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/Product/5
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPut]
        public async Task<ActionResult<ProductDto>> Put([FromBody] ProductEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a Product without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/Product/5
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
