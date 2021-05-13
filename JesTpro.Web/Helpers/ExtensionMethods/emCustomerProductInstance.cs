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
    public static class EmCustomerProductInstance
    {
        public static CustomerProductInstanceDto ToDto(this CustomerProductInstance e, bool loadInners = true)
        {
            if (e == null)
                return null;

            var res = new CustomerProductInstanceDto();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.CostAmount = e.CostAmount;
            res.Discount = e.Discount;
            res.DiscountDescription = e.DiscountDescription;
            res.ExpirationDate = e.ExpirationDate;
            res.IdProductInstance = e.IdProductInstance;
            res.IdCustomer = e.IdCustomer;
            res.IdReversal = e.IdReversal;
            res.ReversalCredit = e.ReversalCredit;
            res.ReversalDate = e.ReversalDate;
            res.Price = e.Price;
            res.IdReceipt = e.IdReceipt;
            res.DiscountType = (DiscountType?)e.DiscountType;
            res.ProductInstance = e.ProductInstance.ToDto(loadInners);
            if (loadInners)
            {
                res.Customer = e.Customer.ToDto();
            }
            if (e.PaymentStatus.HasValue)
            {
                res.PaymentStatus = (Models.PaymentStatus)e.PaymentStatus;
            }
            return res;
        }

        public static CustomerProductInstance ToEntity(this CustomerProductInstanceEditDto e)
        {
            if (e == null)
                return null;

            var res = new CustomerProductInstance();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Name = e.Name;
            res.CostAmount = e.CostAmount;
            res.Discount = e.Discount;
            res.DiscountDescription = e.DiscountDescription;
            res.ExpirationDate = e.ExpirationDate;
            res.IdProductInstance = e.IdProductInstance;
            res.IdCustomer = e.IdCustomer;
            res.IdReversal = e.IdReversal;
            res.ReversalCredit = e.ReversalCredit;
            res.ReversalDate = e.ReversalDate;
            res.IdReceipt = e.IdReceipt;
            res.Price = e.Price;
            res.DiscountType = (int?)e.DiscountType;
            return res;
        }
    }
}
