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
    public class AlarmNotificationController : ControllerBase
    {
        IAlarmNotificationService _service;

        public AlarmNotificationController(IAlarmNotificationService currentService)
        {
            _service = currentService;
        }

        // POST api/AlarmNotification
        [Authorize(Roles = "SuperAdmin,PowerUser,User,Teacher")]
        [HttpPost("SendExpirationMail")]
        public async Task<ActionResult<bool>> SendExpirationMail([FromBody] AlarmNotificationDto alarmNotification)
        {
            var res = await _service.SendExpirationMail(alarmNotification);
            return res;
        }
      
    }
}
