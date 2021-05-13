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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Models
{
    public class UserDto : UserEditDto
    {
       public string RoleName { get; set; }
       public string DashboardData { get; set; }

    }

    public class UserFilterDto : UserEditDto { }
    public class UserEditDto : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Disabled { get; set; }
        public int IdRole { get; set; }
        public string Lang { get; set; }

        public UserEditDto()
        {
        }
    }

    public class ResetPasswordDto
    {
        public string Password { get; set; }
        public string Confirm { get; set; }
        public Guid IdUser { get; set; }
    }
}
