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
    public class SettingController : ControllerBase
    {
        ILogger<SettingController> _logger;
        ISettingService _service;

        public SettingController(ILogger<SettingController> logger, ISettingService currentService)
        {
            _logger = logger;
            _service = currentService;
        }

        // GET api/Setting/GetList
        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetList")]
        public async Task<ActionResult<SettingDto[]>> GetList([FromQuery] String filter)
        {
            var myFilter = new SettingFilterDto();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                myFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingFilterDto>(filter);
            }
            var res = await _service.GetList(myFilter);
            return res;
        }
       

        // PUT api/Setting/5
        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPut]
        public async Task<ActionResult<SettingDto[]>> Put([FromBody] SettingDto[] value)
        {
            var res = await _service.Edit(value);
            return res;
        }

        // GET api/Image/GetList
        [Authorize(Roles = "SuperAdmin, PowerUser,Watcher,User,Teacher")]
        [HttpGet("GetBinaryImage/{id}")]
        public async Task<IActionResult> GetBinaryImage(int id)
        {
            var img = await _service.Get(id);
            if (img == null)
            {
                throw new Exception($"Unable to load image for id=[{id}]");
            }

            if (!System.IO.File.Exists(img.Value))
            {
                throw new Exception($"The image {img.Value} is no longer available");
            }
            var file = new System.IO.FileInfo(img.Value);
            Byte[] b = System.IO.File.ReadAllBytes(img.Value);
            return File(b, MimeTypes.GetMimeType(file.Name));
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,User,Teacher")]
        [HttpPost("UploadImage/{id}"), DisableRequestSizeLimit]
        public async Task<ActionResult<string>> UploadImage(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                throw new Exception("No file selecrted for upload");
            }
            var res = await _service.Upload(Request.Form.Files[0], id);
            return Ok(res);
        }
    }
}
