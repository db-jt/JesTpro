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

using jt.jestpro.dal.Entities;
using jt.jestpro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Helpers.ExtensionMethods
{
    public static class EmUser
    {
        public static UserDto ToDto(this User e)
        {
            if (e == null)
                return null;

            var res = new UserDto();
            res.Id = e.Id;
            res.FirstName = e.FirstName;
            res.LastName = e.LastName;
            res.Email = e.Email;
            res.Disabled = e.Disabled;
            res.UserName = e.UserName;
            res.IdRole = e.IdRole;
            res.RoleName = e.Role != null ? e.Role.Name : "";
            res.DashboardData = e.DashboardData;
            res.Lang = e.Lang;
            return res;
        }

        public static User ToEntity(this UserEditDto e)
        {
            if (e == null)
                return null;

            var res = new User();
            res.Id = e.Id;
            res.FirstName = e.FirstName;
            res.LastName = e.LastName;
            res.Email = e.Email;
            res.Disabled = e.Disabled;
            res.UserName = e.UserName;
            res.IdRole = e.IdRole;
            res.Lang = e.Lang;
            return res;
        }
    }
}
