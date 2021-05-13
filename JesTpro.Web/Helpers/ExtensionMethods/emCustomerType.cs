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
    public static class EmCustomerType
    {
        public static CustomerTypeDto ToDto(this CustomerType e)
        {
            if (e == null)
                return null;

            var res = new CustomerTypeDto();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.CostAmount = e.CostAmount;
            res.Duration = e.Duration;
            return res;
        }

        public static CustomerType ToEntity(this CustomerTypeEditDto e)
        {
            if (e == null)
                return null;

            var res = new CustomerType();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.CostAmount = e.CostAmount;
            res.Duration = e.Duration;
            return res;
        }
    }
}
