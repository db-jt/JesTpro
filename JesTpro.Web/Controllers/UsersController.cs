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
using System;
using jt.jestpro.Services;
using jt.jestpro.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace jt.jestpro.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPost("Add")]
        public async Task<ActionResult<UserDto>> Add([FromBody]UserEditDto user)
        {
            var res = await _userService.Add(user);
            return res;
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<UserDto>> Edit([FromBody]UserEditDto user)
        {
            var res = await _userService.Edit(user);
            return res;
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody]ResetPasswordDto item)
        {
            var res = await _userService.ChangePassword(item);
            return res;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody]ResetPasswordDto item)
        {
            var res = await _userService.ResetPassword(item);
            return res;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModelDto model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            if (user.User.Disabled)
            {
                return BadRequest(new { message = "User disabled. Please contact your administrator" });
            }

            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [HttpGet("LoadAll")]
        public async Task<ActionResult<List<UserDto>>> LoadAll()
        {
            var users = await _userService.LoadAll();
            return users;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser,Watcher")]
        [HttpGet("GetTeacherList")]
        public async Task<ActionResult<List<UserDto>>> GetTeacherList()
        {
            var users = await _userService.GetTeacherList();
            return users;
        }

        [Authorize(Roles = "SuperAdmin,PowerUser")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var res = await _userService.Delete(id);
            return res;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userService.RefreshToken();
            return Ok(user);
        }

        [HttpPost("SaveUserDashboardData")]
        public async Task<ActionResult<string>> SaveUserDashboardData([FromBody] string data)
        {
            var res = await _userService.SaveUserDashboardData(data);
            return res;
        }
    }
}
