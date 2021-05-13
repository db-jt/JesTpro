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
    public static class EmProductSession
    {
        public static ProductSessionDto ToDto(this ProductSession e)
        {
            if (e == null)
                return null;

            var res = new ProductSessionDto();
            res.Id = e.Id;
            res.IdProduct = e.IdProduct;
            res.Description = e.Description;
            res.CreationDate = e.XCreateDate;
            res.IdTeacher = e.IdTeacher;
            res.ProductSessionAttendances = e.ProductSessionAttendances.OrderBy(x => x.CustomerFullName).Select(x => x.ToDto()).ToList();
            res.Teacher = e.Teacher?.ToDto();
            res.IdTeacher = e.IdTeacher;
            return res;
        }

        public static ProductSession ToEntity(this ProductSessionEditDto e)
        {
            if (e == null)
                return null;

            var res = new ProductSession();
            res.Id = e.Id;
            res.Description = e.Description;
            res.IdTeacher = e.IdTeacher;
            res.IdProduct = e.IdProduct;
            return res;
        }
    }
}
