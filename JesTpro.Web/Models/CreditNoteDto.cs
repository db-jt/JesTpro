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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Models
{
    public class CreditNoteDto : CreditNoteEditDto
    {
        
        public DateTime IssueDate { get; set; }
        public string CreditNoteNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public PaymentReceiptDto PaymentReceipt { get; set; }
        public SettingDto[] Settings { get; set; }
        public Guid? IssuedBy { get; set; }
        public virtual UserDto Owner { get; set; }

        public CreditNoteDto()
        {
        }
    }

    public class CreditNoteFilterDto : CreditNoteEditDto
    {
    }

    public class CreditNoteEditDto : BaseDto
    {
        public Guid IdReceipt { get; set; }
        public string Description { get; set; }
        public int? Year { get; set; }

    }
}
