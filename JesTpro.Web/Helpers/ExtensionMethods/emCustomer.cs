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
    public static class EmCustomer
    {
        public static CustomerDto ToDto(this Customer e, bool includeReceipts = true)
        {
            if (e == null)
                return null;

            var res = new CustomerDto();
            res.Id = e.Id;
            res.Address = e.Address;
            res.HouseNumber = e.HouseNumber;
            res.City = e.City;
            res.Country = e.Country;
            res.State = e.State;
            res.PostalCode = e.PostalCode;
            res.BirthDate = e.BirthDate;
            res.FirstName = e.FirstName;
            res.FiscalCode = e.FiscalCode;
            res.IdType = e.IdType;
            res.LastName = e.LastName;
            res.MembershipFee = e.MembershipFee;
            res.MembershipFeeExpiryDate = e.MembershipFeeExpiryDate;
            res.MembershipLastPayDate = e.MembershipLastPayDate;
            res.Note = e.Note;
            res.TutorBirthDate = e.TutorBirthDate;
            res.TutorFirstName = e.TutorFirstName;
            res.TutorFiscalCode = e.TutorFiscalCode;
            res.TutorLastName = e.TutorLastName;
            res.CustomerType = e.CustomerType.ToDto();
            res.PhoneNumber = e.PhoneNumber;
            res.PhoneNumberAlternative = e.PhoneNumberAlternative;
            res.TutorPhoneNumber = e.TutorPhoneNumber;
            res.Email = e.Email;
            res.TutorEmail = e.TutorEmail;
            res.TutorType = e.TutorType;
            res.FullName = e.FullName;
            res.Gender = e.Gender;
            res.BirthPlace = e.BirthPlace;
            res.BirthProvince = e.BirthProvince;
            res.MedicalCertificateExpiration = e.MedicalCertificateExpiration;
            res.Photo = e.Photo;
            if (includeReceipts)
            {
                res.PaymentReceipts = e.PaymentReceipts.Select(x => x.ToDto()).ToList();
            }
            return res;
        }

        public static Customer ToEntity(this CustomerEditDto e)
        {
            if (e == null)
                return null;

            var res = new Customer();
            res.Id = e.Id;
            res.Address = e.Address;
            res.HouseNumber = e.HouseNumber;
            res.City = e.City;
            res.Country = e.Country;
            res.State = e.State;
            res.PostalCode = e.PostalCode;
            res.BirthDate = e.BirthDate;
            res.FirstName = e.FirstName;
            res.FiscalCode = e.FiscalCode;
            res.IdType = e.IdType;
            res.LastName = e.LastName;
            res.MembershipFee = e.MembershipFee;
            res.MembershipFeeExpiryDate = e.MembershipFeeExpiryDate;
            res.MembershipLastPayDate = e.MembershipLastPayDate;
            res.Note = res.Note;
            res.TutorBirthDate = e.TutorBirthDate;
            res.TutorFirstName = e.TutorFirstName;
            res.TutorFiscalCode = e.TutorFiscalCode;
            res.TutorLastName = e.TutorLastName;
            res.PhoneNumber = e.PhoneNumber;
            res.PhoneNumberAlternative = e.PhoneNumberAlternative;
            res.TutorPhoneNumber = e.TutorPhoneNumber;
            res.Email = e.Email;
            res.TutorEmail = e.TutorEmail;
            res.TutorType = e.TutorType;
            res.Gender = e.Gender;
            res.BirthPlace = e.BirthPlace;
            res.BirthProvince = e.BirthProvince;
            res.FullName = $"{e.LastName} {e.FirstName}";
            res.MedicalCertificateExpiration = e.MedicalCertificateExpiration;
            return res;
        }
    }
}
