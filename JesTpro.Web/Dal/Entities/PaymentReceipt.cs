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

namespace jt.jestpro.dal.Entities
{
    public class PaymentReceipt : BaseEntity
    {
        /* 
         CREATE TABLE `payment_receipt` (
              `Id` char(36) NOT NULL,
              `IdCustomer` char(36) NOT NULL,
              `PaymentDate` datetime NOT NULL,
              `Amount` decimal(15,2) NOT NULL,
              `RawData` longtext,
              `ReceiptPath` varchar(512) DEFAULT NULL,
              `XCreateDate` datetime DEFAULT NULL,
              `XUpdateDate` datetime DEFAULT NULL,
              `XDeleteDate` datetime DEFAULT NULL,
              `XLastEditUser` varchar(45) DEFAULT NULL,
              `XCreationUser` varchar(45) DEFAULT NULL,
              PRIMARY KEY (`Id`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8;


         */

        public Guid? IdCustomer { get; set; }
        public virtual Customer Customer { get; set;}
        public DateTime IssueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal CostAmount { get; set; }
        public string ReceiptPath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public virtual ICollection<PaymentReceiptDetail> PaymentReceiptDetails { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerFiscalCode { get; set; }
        public virtual ICollection<CreditNote> CreditNotes { get; set; }
        public Guid? IssuedBy { get; set; }
        public string PaymentType { get; set; }
        public virtual User Owner { get; set; }

        public PaymentReceipt()
        {
            PaymentReceiptDetails = new List<PaymentReceiptDetail>();
            CreditNotes = new List<CreditNote>();
        }
    }
}
