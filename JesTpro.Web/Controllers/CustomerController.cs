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
    public class CustomerController : ControllerBase
    {
        ILogger<CustomerController> _logger;
        ICustomerService _service;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/Customer/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<CustomerDto[]>> GetList([FromQuery] CustomerFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Customer/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetListSimpleFilter")]
        public async Task<ActionResult<CustomerDto[]>> GetListSimpleFilter([FromQuery] string filter)
        {
            var res = await _service.GetListSimpleFilter(filter);
            return res;
        }

        // GET api/Customer/GetListFiltered
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<CustomerDto[]>> GetListFiltered([FromBody] CustomerFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Customer/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/Customer
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Post([FromBody] CustomerEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/Customer/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<CustomerDto>> Put([FromBody] CustomerEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a Customer without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/Customer/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser, Watcher, User,Teacher")]
        [HttpGet("GetExpiredFees")]
        public async Task<ActionResult<CustomerDto[]>> GetExpiredFees()
        {
            var res = await _service.GetExpiredFees();
            return res;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser, Watcher, User,Teacher")]
        [HttpGet("GetExpiredMedicalCertificates")]
        public async Task<ActionResult<CustomerDto[]>> GetExpiredMedicalCertificates()
        {
            var res = await _service.GetExpiredMedicalCertificates();
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetListPaginated")]
        public async Task<ActionResult<CustomerPaginatedDto>> GetListPaginated([FromBody] CustomerFilterDto filter)
        {
            var res = await _service.GetListPaginated(filter);
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpPost("GetProductCustomers/{idProduct}")]
        public async Task<ActionResult<CustomerPaginatedDto>> GetProductCustomers([FromBody] BasePaginatedFilterDto filter, Guid idProduct)
        {
            var res = await _service.GetProductCustomers(filter, idProduct);
            return res;
        }

        [HttpGet("GetBinaryImage/{id}")]
        public async Task<IActionResult> GetBinaryImage(Guid id)
        {
            var img = await _service.GetImageFullPath(id);
            var file = new FileInfo(img);
            byte[] b = System.IO.File.ReadAllBytes(img);
            return File(b, MimeTypes.GetMimeType(file.Name));
        }

        [HttpPost("UploadPhoto/{id}"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> UploadPhoto(Guid id)
        {
            if (Request.Form.Files.Count == 0)
            {
                throw new Exception("No file selected for upload");
            }
            var res = await _service.UploadPhoto(Request.Form.Files[0], id);
            return Ok(res);
        }
    }
}
