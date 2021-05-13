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
    public class AttachmentController : ControllerBase
    {
        ILogger<AttachmentController> _logger;
        IAttachmentService _service;

        public AttachmentController(ILogger<AttachmentController> logger, IAttachmentService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/Attachment/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList/{idResource}")]
        public async Task<ActionResult<AttachmentDto[]>> GetList(Guid idResource)
        {
            var res = await _service.GetList(new AttachmentFilterDto() { IdResource = idResource });
            return res;
        }


        // GET api/Attachment/5
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AttachmentDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/Attachment
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<AttachmentDto>> Post([FromBody] AttachmentEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        // PUT api/Attachment/5
        [Authorize(Roles = "SuperAdmin,PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<AttachmentDto>> Put([FromBody] AttachmentEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a Attachment without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/Attachment/5
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
            var img = await _service.GetFullPath(id);
            var file = new FileInfo(img);
            byte[] b = System.IO.File.ReadAllBytes(img);
            return File(b, MimeTypes.GetMimeType(file.Name));
        }

        [HttpPost("Upload/{idResource}"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> Upload(Guid idResource)
        {
            if (Request.Form.Files.Count == 0)
            {
                throw new Exception("No file selected for upload");
            }
            var res = await _service.Upload(Request.Form.Files[0], idResource);
            return Ok(res);
        }
    }
}
