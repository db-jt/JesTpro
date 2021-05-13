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
    public class MassiveRequestController : ControllerBase
    {
        ILogger<MassiveRequestController> _logger;
        IMassiveRequestService _service;

        public MassiveRequestController(ILogger<MassiveRequestController> logger, IMassiveRequestService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/MassiveRequest/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<MassiveRequestDto[]>> GetList([FromQuery] MassiveRequestFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }


        // GET api/MassiveRequest/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MassiveRequestDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/MassiveRequest
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<MassiveRequestDto>> Post([FromBody] MassiveRequestEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/MassiveRequest/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<MassiveRequestDto>> Put([FromBody] MassiveRequestEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a MassiveRequest without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/MassiveRequest/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }


        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var img = await _service.Get(id);
            var file = new FileInfo(img.FileToImport);
            byte[] b = System.IO.File.ReadAllBytes(img.FileToImport);
            return File(b, MimeTypes.GetMimeType(file.Name));
        }

        [HttpPost("Upload/{importType}"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> Upload(ImportType importType)
        {
            if (Request.Form.Files.Count == 0)
            {
                throw new Exception("No file selected for upload");
            }
            var res = await _service.Upload(Request.Form.Files[0], importType);
            return Ok(res);
        }
    }
}
