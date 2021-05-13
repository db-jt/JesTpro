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
    public class PaymentReceiptDto : PaymentReceiptEditDto
    {
        public CustomerDto Customer { get; set; }
        public string InvoiceNumber { get; set; }
        public CreditNoteDto CreditNote { get; set; }
        public Guid? IssuedBy { get; set; }
        public UserDto Owner { get; set; }

        public PaymentReceiptDto()
        {
            Customer = new CustomerDto();
            PaymentReceiptDetails = new List<PaymentReceiptDetailDto>();
        }
    }

    public class PaymentReceiptFilterDto : PaymentReceiptEditDto
    {
        public bool? WorkableOnly { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class PaymentReceiptEditDto : BaseDto
    {
        public Guid? IdCustomer { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal CostAmount { get; set; }
        public string ReceiptPath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaymentType { get; set; }
        public ICollection<PaymentReceiptDetailDto> PaymentReceiptDetails { get; set; }
        public PaymentReceiptEditDto()
        {
            PaymentReceiptDetails = new List<PaymentReceiptDetailDto>();
        }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerFiscalCode { get; set; }

    }

    public class PaymentReceiptEditForFastInvoceDto: PaymentReceiptEditDto
    {
        public decimal? Discount { get; set; }
        public string DiscountDescription { get; set; }
        public DiscountType? DiscountType { get; set; }
    }

    public class InvoiceRequestData
    {
        [Required(ErrorMessage = "required")]
        public Guid IdReceipt { get; set; }

        [Required(ErrorMessage = "required")]
        [MaxLength(256)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "required")]
        [MaxLength(45)]
        public string CustomerFiscalCode { get; set; }

        [Required(ErrorMessage = "required")]
        [MaxLength(512)]
        public string CustomerAddress { get; set; }

        public int? Year { get; set; }
        public string PaymentType { get; set; }
        public string Description { get; set; }

        public decimal? Discount { get; set; }
        public string DiscountDescription { get; set; }
        public DiscountType? DiscountType { get; set; }
    }

}
