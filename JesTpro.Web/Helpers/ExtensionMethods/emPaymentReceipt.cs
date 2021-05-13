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
    public static class EmPaymentReceipt
    {
        public static PaymentReceiptDto ToDto(this PaymentReceipt e)
        {
            if (e == null)
                return null;

            var res = new PaymentReceiptDto();
            res.Id = e.Id;
            res.IdCustomer = e.IdCustomer;
            res.PaymentDate = e.PaymentDate;
            res.IssueDate = e.IssueDate;
            res.ReceiptPath = e.ReceiptPath;
            res.CostAmount = e.CostAmount;
            res.Name = e.Name;
            res.Description = e.Description;
            res.Customer = e.Customer.ToDto(false);
            res.InvoiceNumber = e.InvoiceNumber;
            res.PaymentReceiptDetails = e.PaymentReceiptDetails.OrderByDescending(x => x.XCreateDate).Select(x => x.ToDto()).ToArray();
            res.CustomerAddress = e.CustomerAddress;
            res.CustomerFiscalCode = e.CustomerFiscalCode;
            res.CustomerName = e.CustomerName;
            res.CreditNote = e.CreditNotes.Count() > 0 ? e.CreditNotes.ToArray()[0].ToDto() : null;
            res.IssuedBy = e.IssuedBy;
            res.Owner = e.Owner != null ? e.Owner.ToDto() : null;
            res.PaymentType = e.PaymentType;
            return res;
        }

        public static PaymentReceipt ToEntity(this PaymentReceiptEditDto e)
        {
            if (e == null)
                return null;

            var res = new PaymentReceipt();
            res.Id = e.Id;
            res.IdCustomer = e.IdCustomer;
            res.PaymentDate = e.PaymentDate;
            res.IssueDate = e.IssueDate;
            res.ReceiptPath = e.ReceiptPath;
            //res.CostAmount = e.CostAmount;
            res.Name = e.Name;
            res.Description = e.Description;
            res.PaymentType = e.PaymentType;
            return res;
        }

        //public static PaymentReceiptViewModel ToViewModel(this PaymentReceiptDto e)
        //{
        //    if (e == null)
        //        return null;

        //    var res = new PaymentReceiptViewModel();
        //    res.Id = e.Id;
        //    res.IdCustomer = e.IdCustomer;
        //    res.PaymentDate = e.PaymentDate;
        //    res.IssueDate = e.IssueDate;
        //    res.ReceiptPath = e.ReceiptPath;
        //    res.Amount = e.Amount;
        //    res.Name = e.Name;
        //    res.Description = e.Description;
        //    res.InvoiceNumber = e.InvoiceNumber;
        //    res.PaymentReceiptDetails = e.PaymentReceiptDetails.Select(x => x.ToViewModel()).ToArray();
        //    res.CustomerAddress = e.CustomerAddress;
        //    res.CustomerFiscalCode = e.CustomerFiscalCode;
        //    res.CustomerName = e.CustomerName;
        //    res.CustomerDBName = $"{e.Customer.FirstName} {e.Customer.LastName}";
        //    return res;
        //}
    }
}
