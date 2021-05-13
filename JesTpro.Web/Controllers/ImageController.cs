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
    public class ImageController : ControllerBase
    {
        ILogger<ImageController> _logger;
        IImageService _service;

        public ImageController(ILogger<ImageController> logger, IImageService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpPost("UploadMulti"), DisableRequestSizeLimit]
        public async Task<ActionResult<ImageDto[]>> UploadMulti()
        {
            var res = await _service.UploadMulti(Request.Form.Files);
            return res;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpPost("Overwrite/{idImage}"), DisableRequestSizeLimit]
        public async Task<ActionResult<ImageDto>> Overwrite(Guid idImage)
        {
            var res = await _service.OverWrite(Request.Form.Files[0], idImage);
            return res;
        }

        // GET api/Image/GetList
        [Authorize(Roles = "SuperAdmin, PowerUser,Watcher,User")]
        [HttpGet("GetList")]
        public async Task<ActionResult<ImageDto[]>> GetList([FromQuery] ImageFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }
        // GET api/Image/GetList
        [Authorize(Roles = "SuperAdmin, PowerUser,Watcher,User")]
        [HttpGet("GetBinaryImage/{id}")]
        public async Task<IActionResult> GetBinaryImage(Guid id)
        {
            var img = await _service.Get(id);
            if (img == null)
            {
                throw new Exception($"Unable to load image for id=[{id}]");
            }

            if (!System.IO.File.Exists(img.Path))
            {
                throw new Exception($"The image {img.Path} is no longer available");
            }
            var file = new System.IO.FileInfo(img.Path);
            Byte[] b = System.IO.File.ReadAllBytes(img.Path);       
            return File(b, MimeTypes.GetMimeType(file.Name));
        }

        [Authorize(Roles = "SuperAdmin, PowerUser,Watcher,User")]
        [HttpPost("GetListFiltered")]
        public async Task<ActionResult<ImageDto[]>> GetListFiltered([FromBody] ImageFilterDto filter)
        {
            var res = await _service.GetList(filter);
            return res;
        }

        // GET api/Image/5
        [Authorize(Roles = "SuperAdmin, PowerUser,Watcher,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageDto>> Get(Guid id)
        {
            var res = await _service.Get(id);
            return res;
        }

        // POST api/Image
        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpPost]
        public async Task<ActionResult<ImageDto>> Post([FromBody] ImageEditDto value)
        {
            var res = await _service.Save(value);
            return res;
        }

        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpPost("SetDefault")]
        public async Task<ActionResult<ImageDto[]>> SetDefault([FromBody] ImageEditDto value)
        {
            var res = await _service.SetDefault(value);
            return res;
        }

        // PUT api/Image/5
        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpPut]
        public async Task<ActionResult<ImageDto>> Put([FromBody] ImageEditDto value)
        {
            if (value.Id == Guid.Empty)
            {
                throw new Exception("Unable to edit a Image without ID");
            }
            var res = await _service.Save(value);
            return res;
        }

        // DELETE api/Image/5
        [Authorize(Roles = "SuperAdmin, PowerUser,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return res;
        }
    }
}
