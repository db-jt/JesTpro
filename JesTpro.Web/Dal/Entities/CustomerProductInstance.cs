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
    public enum PaymentStatus
    {
        None,
        WaitingForPayment,
        Completed,
        Aborted
    }
    public class CustomerProductInstance : BaseEntity
    {
        public Guid IdCustomer { get; set; }
        public virtual Customer Customer { get; set; }
        public Guid IdProductInstance { get; set; }
        public Guid? IdReceipt { get; set; }
        public virtual ProductInstance ProductInstance { get; set; }

        public decimal CostAmount { get; set; }
        public decimal? Discount { get; set; }
        public string DiscountDescription { get; set; }
        public Guid? IdReversal { get; set; }
        public DateTime? ReversalDate { get; set; }
        public decimal? ReversalCredit { get; set; }
        public string ReversalDescription { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? DiscountType { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public CustomerProductInstance()
        {
        }
    }
}
