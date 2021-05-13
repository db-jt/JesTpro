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
    public static class EmProductInstance
    {
        public static ProductInstanceDto ToDto(this ProductInstance e, bool loadProduct = true)
        {
            if (e == null)
                return null;

            var res = new ProductInstanceDto();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.IdProduct = e.IdProduct;
            res.Months = e.Months;
            res.Price = e.Price;
            res.Weeks = e.Weeks;
            res.Years = e.Years;
            res.Days = e.Days;
            if (loadProduct)
            {
                res.Product = e.Product.ToDto(loadProduct);
            }
            return res;
        }

        public static ProductInstance ToEntity(this ProductInstanceEditDto e)
        {
            if (e == null)
                return null;

            var res = new ProductInstance();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.IdProduct = e.IdProduct;
            res.Months = e.Months;
            res.Price = e.Price;
            res.Weeks = e.Weeks;
            res.Years = e.Years;
            res.Days = e.Days;
            return res;
        }
    }
}
