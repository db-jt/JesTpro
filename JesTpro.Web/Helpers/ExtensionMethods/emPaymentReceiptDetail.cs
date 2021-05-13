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
    public static class EmPaymentReceiptDetail
    {
        public static PaymentReceiptDetailDto ToDto(this PaymentReceiptDetail e)
        {
            if (e == null)
                return null;

            var res = new PaymentReceiptDetailDto();
            res.Id = e.Id;
            res.IdResource = e.IdResource;
            res.CostAmount = e.CostAmount;
            res.Name = e.Name;
            res.IdReceipt = e.IdReceipt;
            res.ReceiptDetailType = (Models.ReceiptDetailType)e.ReceiptDetailType;
            res.Description = e.Description;
            res.ProductAmount = e.ProductAmount;
            return res;
        }

        public static PaymentReceiptDetail ToEntity(this PaymentReceiptDetailEditDto e)
        {
            if (e == null)
                return null;

            var res = new PaymentReceiptDetail();
            res.Id = e.Id;
            res.IdResource = e.IdResource;
            res.IdReceipt = e.IdReceipt;
            res.ReceiptDetailType = (dal.Entities.ReceiptDetailType)e.ReceiptDetailType;
            res.CostAmount = e.CostAmount * e.ProductAmount;
            res.Name = e.Name;
            res.Description = e.Description;
            res.ProductAmount = e.ProductAmount;
            return res;
        }

        //public static PaymentReceiptDetailViewModel ToViewModel(this PaymentReceiptDetailDto e)
        //{
        //    if (e == null)
        //        return null;

        //    var res = new PaymentReceiptDetailViewModel();
        //    res.Id = e.Id;
        //    res.IdResource = e.IdResource;
        //    res.CostAmount = e.CostAmount;
        //    res.Name = e.Name;
        //    res.IdReceipt = e.IdReceipt;
        //    res.ReceiptDetailType = (ViewModels.ReceiptDetailType)e.ReceiptDetailType;
        //    res.Description = e.Description;
        //    res.ProductAmount = e.ProductAmount;
        //    return res;
        //}
    }
}
