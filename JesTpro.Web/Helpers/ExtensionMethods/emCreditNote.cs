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
    public static class EmCreditNote
    {
        public static CreditNoteDto ToDto(this CreditNote e, bool includeReceipt = false)
        {
            if (e == null)
                return null;

            var res = new CreditNoteDto();
            res.Id = e.Id;
            res.Description = e.Description;
            res.InvoiceNumber = e.InvoiceNumber;
            res.CreditNoteNumber = e.CreditNoteNumber;
            res.IdReceipt = e.IdReceipt;
            res.InvoiceDate = e.InvoiceDate;
            res.IssueDate = e.IssueDate;
            res.IssuedBy = e.IssuedBy;
            res.Owner = e.Owner != null ? e.Owner.ToDto() : null;
            if (includeReceipt)
            {
                res.PaymentReceipt = e.PaymentReceipt.ToDto();
            }
            return res;
        }

        public static CreditNote ToEntity(this CreditNoteEditDto e)
        {
            if (e == null)
                return null;

            var res = new CreditNote();
            res.Id = e.Id;
            res.Description = e.Description;
            res.IdReceipt = e.IdReceipt;
            return res;
        }
    }
}
