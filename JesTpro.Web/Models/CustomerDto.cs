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

namespace jt.jestpro.Models
{
    public class CustomerDto : CustomerEditDto
    {
        public CustomerTypeDto CustomerType { get; set; }
        public List<CustomerProductInstanceDto> CustomerProductInstances { get; set; }
        public List<PaymentReceiptDto> PaymentReceipts { get; set; }
        public string FullName { get; set; }
        public string Photo { get; set; }
        public CustomerDto()
        {
            CustomerProductInstances = new List<CustomerProductInstanceDto>();
            PaymentReceipts = new List<PaymentReceiptDto>();
        }
    }

    public class CustomerFilterDto : BasePaginatedFilterDto
    {
        public bool OnlyExpiredFees { get; set; }
        public bool OnlyExpiredMedicalCertificates { get; set; }
    }
    public class CustomerEditDto : BaseDto
    {
        public Guid IdType { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string FiscalCode { get; set; }
        public string Address { get; set; }
        public string HouseNumber { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string BirthProvince { get; set; }
        public string TutorFirstName { get; set; }
        public string TutorLastName { get; set; }
        public string TutorFiscalCode { get; set; }
        public DateTime? TutorBirthDate { get; set; }
        public string Note { get; set; }
        public decimal MembershipFee { get; set; }
        public DateTime? MembershipLastPayDate { get; set; }
        public DateTime? MembershipFeeExpiryDate { get; set; }
        public string Email { get; set; }
        public DateTime? MedicalCertificateExpiration { get; set; }
        public string TutorEmail { get; set; }

        public string PhoneNumber { get; set; }
        public string PhoneNumberAlternative { get; set; }
        public string TutorPhoneNumber { get; set; }
        public string TutorType { get; set; }
    }

    public class CustomerPaginatedDto
    {
        public int TotalItems { get; set; }
        public CustomerDto[] Customers { get; set; }

        public CustomerPaginatedDto()
        {
            Customers = new CustomerDto[0];
        }
    }
}
