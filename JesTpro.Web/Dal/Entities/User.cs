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

namespace jt.jestpro.dal.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public bool Disabled { get; set; }
        public int IdRole { get; set; }
        public virtual Role Role { get; set; }
        public string DashboardData { get; set; }
        public string Lang { get; set; }
        public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; }
        public virtual ICollection<CreditNote> CreditNotes { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductSession> ProductSessions { get; set; }

        public User()
        {
            PaymentReceipts = new List<PaymentReceipt>();
            CreditNotes = new List<CreditNote>();
            Products = new List<Product>();
            ProductSessions = new List<ProductSession>();
        }
    }
}