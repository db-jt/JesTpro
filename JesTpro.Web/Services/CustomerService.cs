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

using jt.jestpro.dal;
using jt.jestpro.dal.Entities;
using jt.jestpro.Helpers;
using jt.jestpro.Helpers.ExtensionMethods;
using jt.jestpro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface ICustomerService : ICRUDService<CustomerDto, CustomerFilterDto, CustomerEditDto>
    {
        Task<CustomerDto[]> GetListSimpleFilter(string filter);
        Task<CustomerDto[]> GetExpiredFees();
        Task<CustomerDto[]> GetExpiredMedicalCertificates();
        Task<CustomerPaginatedDto> GetListPaginated(CustomerFilterDto customerFilter);
        Task<string> UploadPhoto(IFormFile file, Guid id);
        Task<string> GetImageFullPath(Guid id);
        Task<CustomerPaginatedDto> GetProductCustomers(BasePaginatedFilterDto pageFilter, Guid idProduct);
    }
    public class CustomerService : ICustomerService
    {
        MyDBContext _dbCtx;
        ILogger<CustomerService> _logger;
        IOptions<AppSettings> _appSettings;

        public CustomerService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<CustomerService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete Customer for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Customers.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<CustomerDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get Customer for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto(false);
        }

        private async Task<Customer> GetInner(Guid id)
        {
            return await _dbCtx.Customers.FindAsync(id);
        }

        public async Task<CustomerDto[]> GetList(CustomerFilterDto filter)
        {
            _logger.LogDebug($"Calling getList Customer");

            IQueryable<Customer> query = _dbCtx.Customers;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Take(100).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        private void CheckDuplicatedFiscalCode(string fiscalCode, Guid id)
        {
            if (string.IsNullOrWhiteSpace(fiscalCode)){
                return;
            }

            var item = _dbCtx.Customers.FirstOrDefault(x => x.FiscalCode.Equals(fiscalCode));
            if (item == null)
            {
                return;
            }

            if ( (id != Guid.Empty && item.Id != id) || (id == Guid.Empty && item != null) )
            {
                throw new Exception($"A customer already exists with fiscal code '{fiscalCode}' ");
            }
            return;
        }

        public async Task<CustomerDto> Save(CustomerEditDto dto)
        {
            Customer res;
            CheckDuplicatedFiscalCode(dto.FiscalCode, dto.Id);
            if (dto.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update Customer for id=[{dto.Id}]");
                //edit
                res = await this.GetInner(dto.Id);
                if (res == null)
                {
                    throw new NotFoundException($"Customer with id={dto.Id} not exists!");
                }
                res.Address = dto.Address;
                res.HouseNumber = dto.HouseNumber;
                res.BirthDate = dto.BirthDate;
                res.IdType = dto.IdType;
                res.LastName = dto.LastName;
                res.Email = dto.Email;
                res.FirstName = dto.FirstName;
                res.FiscalCode = dto.FiscalCode;
                res.MembershipFee = dto.MembershipFee;
                res.MembershipFeeExpiryDate = dto.MembershipFeeExpiryDate;
                res.MembershipLastPayDate = dto.MembershipLastPayDate;
                res.Note = dto.Note;
                res.PhoneNumber = dto.PhoneNumber;
                res.PhoneNumberAlternative = dto.PhoneNumberAlternative;
                res.TutorBirthDate = dto.TutorBirthDate;
                res.TutorEmail = dto.TutorEmail;
                res.TutorFirstName = dto.TutorFirstName;
                res.TutorFiscalCode = dto.TutorFiscalCode;
                res.TutorLastName = dto.TutorLastName;
                res.TutorPhoneNumber = dto.TutorPhoneNumber;
                res.TutorType = dto.TutorType;
                res.City = dto.City;
                res.Country = dto.Country;
                res.State = dto.State;
                res.PostalCode = dto.PostalCode;
                res.MedicalCertificateExpiration = dto.MedicalCertificateExpiration;
                res.Gender = dto.Gender;
                res.BirthPlace = dto.BirthPlace;
                res.BirthProvince = dto.BirthProvince;
                res.FullName = $"{dto.LastName} {dto.FirstName}";

                 _dbCtx.Customers.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = dto.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert Customer for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.Customers.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

        public async Task<CustomerDto[]> GetListSimpleFilter(string filter)
        {
            _logger.LogDebug($"Calling getListSimpleFilter Customer");
            filter = filter.ToLower();
            var result = await _dbCtx.Customers.Where(x => x.FirstName.ToLower().Contains(filter)
            || x.LastName.ToLower().Contains(filter)
            || x.Email.ToLower().Contains(filter)
            || x.TutorFirstName.ToLower().Contains(filter)
            || x.TutorLastName.ToLower().Contains(filter)
            || x.TutorEmail.ToLower().Contains(filter)
            ).ToArrayAsync();
            return result.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Take(100).Select(x => x.ToDto()).ToArray();
        }

        private async Task<int> GetMonthTreshold()
        {
            var setting = await _dbCtx.Settings.FirstOrDefaultAsync(x => x.Key.Equals("company.expiryMonthLimit"));
            if (setting != null && int.TryParse(setting.Value, out int month))
            {
                return month;
            }
            else
            {
                _logger.LogWarning("No value found for company.expiryMonthLimit=" + setting == null ? "settingObj-null" : setting.Value);
                return 2; //default value
            }
        }

        public async Task<CustomerDto[]> GetExpiredFees()
        {
            var month = await GetMonthTreshold();
            var date = DateTime.UtcNow.AddMonths(month);
            var res = await _dbCtx.Customers.Where(x => !x.MembershipFeeExpiryDate.HasValue || x.MembershipFeeExpiryDate <= date).OrderBy(row => row.MembershipFeeExpiryDate ?? DateTime.MinValue).ToArrayAsync();
            return res.Select(x => x.ToDto(false)).ToArray();
        }

        public async Task<CustomerDto[]> GetExpiredMedicalCertificates()
        {
            var month = await GetMonthTreshold();
            var date = DateTime.UtcNow.AddMonths(month);
            var res = await _dbCtx.Customers.Where(x => !x.MedicalCertificateExpiration.HasValue || x.MedicalCertificateExpiration <= date).OrderBy(row => row.MedicalCertificateExpiration ?? DateTime.MinValue).ToArrayAsync();
            return res.Select(x => x.ToDto(false)).ToArray();
        }

        public async Task<CustomerPaginatedDto> GetListPaginated(CustomerFilterDto customerFilter)
        {
            _logger.LogDebug($"Calling GetListPaginated Customer");
            var loadReceipts = true;

            IQueryable<Customer> query = _dbCtx.Customers;
            if (!customerFilter.PageNumber.HasValue)
            {
                customerFilter.PageNumber = 0;
            }
            if (!customerFilter.PageSize.HasValue)
            {
                customerFilter.PageSize = 10;
            }
            if (!string.IsNullOrWhiteSpace(customerFilter.Filter))
            {
                query = query.Where(x => x.FullName.ToLower().Contains(customerFilter.Filter.ToLower()));
            }

            if (customerFilter.OnlyExpiredMedicalCertificates)
            {
                var month = await GetMonthTreshold();
                var date = DateTime.UtcNow.AddMonths(month);
                loadReceipts = false;
                query = query.Where(x => !x.MedicalCertificateExpiration.HasValue || x.MedicalCertificateExpiration <= date).OrderBy(row => row.MedicalCertificateExpiration ?? DateTime.MinValue);
            }

            if (customerFilter.OnlyExpiredFees)
            {
                var month = await GetMonthTreshold();
                var date = DateTime.UtcNow.AddMonths(month);
                loadReceipts = false;
                query = query.Where(x => !x.MembershipFeeExpiryDate.HasValue || x.MembershipFeeExpiryDate <= date).OrderBy(row => row.MembershipFeeExpiryDate ?? DateTime.MinValue);
            }

            if (!string.IsNullOrWhiteSpace(customerFilter.SortColumn))
            {
                if (customerFilter.SortColumn == "email")
                {
                    query = customerFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email);
                }
                else if (customerFilter.SortColumn == "birthDate")
                {
                    query = customerFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.BirthDate) : query.OrderBy(x => x.BirthDate);
                }
                else if (customerFilter.SortColumn == "medicalCertificateExpiration")
                {
                    query = customerFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.MedicalCertificateExpiration) : query.OrderBy(x => x.MedicalCertificateExpiration);
                }
                else if (customerFilter.SortColumn == "expirationDate")
                {
                    query = customerFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.MembershipFeeExpiryDate) : query.OrderBy(x => x.MembershipFeeExpiryDate);
                }
                else
                {
                    // deafult ord by fullname
                    query = customerFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
                }

            }
            else
            {
                //deafult order
                query = query.OrderBy(x => x.FullName);
            }

            var result = new CustomerPaginatedDto();
            result.TotalItems = query.Count();
            var customers = await query.Skip(customerFilter.PageSize.Value * customerFilter.PageNumber.Value).Take(customerFilter.PageSize.Value).ToArrayAsync();
            result.Customers = customers.Select(x => x.ToDto(loadReceipts)).ToArray();

            return result;
        }

        public async Task<string> UploadPhoto(IFormFile file, Guid id)
        {
            var item = await GetInner(id);
            if (item == null)
            {
                throw new System.Exception($"No cutomer fuond with id={id}");
            }

            var pathToSave = Path.Combine(_appSettings.Value.ImagePath, "customers", id.ToString()); //TODO: maybe savnig the user full name shuold be usefull but I wanto to keep away from strange chars..
            Directory.CreateDirectory(pathToSave);

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            item.Photo = fileName;
            item.PhotoFullPath = fullPath;

            await _dbCtx.SaveChangesAsync();
            return fullPath;
        }

        public async Task<string> GetImageFullPath(Guid id)
        {
            var item = await GetInner(id);
            if (item == null)
            {
                throw new System.Exception($"No cutomer fuond with id={id}");
            }
            if (string.IsNullOrWhiteSpace(item.PhotoFullPath))
            {
                throw new Exception($"No photo found for customer {item.FullName}");
            }
            if (!System.IO.File.Exists(item.PhotoFullPath))
            {
                throw new Exception($"The image {item.PhotoFullPath} is no longer available");
            }
            return item.PhotoFullPath;
        }

        public async Task<CustomerPaginatedDto> GetProductCustomers(BasePaginatedFilterDto pageFilter, Guid idProduct)
        {
            _logger.LogDebug($"Calling GetListPaginated Customer");

            IQueryable<Customer> query = _dbCtx.Customers;
            if (!pageFilter.PageNumber.HasValue)
            {
                pageFilter.PageNumber = 0;
            }
            if (!pageFilter.PageSize.HasValue)
            {
                pageFilter.PageSize = 10;
            }
            if (!string.IsNullOrWhiteSpace(pageFilter.Filter))
            {
                query = query.Where(x => x.FullName.ToLower().Contains(pageFilter.Filter.ToLower()));
            }
            var product = await _dbCtx.Products.FindAsync(idProduct);
            var now = product.EndDate.HasValue ? product.EndDate.Value : DateTime.UtcNow;
            query = query.Where(x => x.CustomerProductInstances.Any(y => y.ProductInstance.IdProduct == idProduct));
            query = query.Where(x => x.CustomerProductInstances.Any(y => y.ExpirationDate.HasValue && y.ExpirationDate.Value <= now));

            if (!string.IsNullOrWhiteSpace(pageFilter.SortColumn))
            {
                if (pageFilter.SortColumn == "email")
                {
                    query = pageFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email);
                }
                else if (pageFilter.SortColumn == "birthDate")
                {
                    query = pageFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.BirthDate) : query.OrderBy(x => x.BirthDate);
                }
                else if (pageFilter.SortColumn == "medicalCertificateExpiration")
                {
                    query = pageFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.MedicalCertificateExpiration) : query.OrderBy(x => x.MedicalCertificateExpiration);
                }
                else if (pageFilter.SortColumn == "expirationDate")
                {
                    query = pageFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.MembershipFeeExpiryDate) : query.OrderBy(x => x.MembershipFeeExpiryDate);
                }
                else
                {
                    // deafult ord by fullname
                    query = pageFilter.SortOrder == "desc" ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
                }

            }
            else
            {
                //deafult order
                query = query.OrderBy(x => x.FullName);
            }

            var result = new CustomerPaginatedDto();
            result.TotalItems = query.Count();
            var customers = await query.Skip(pageFilter.PageSize.Value * pageFilter.PageNumber.Value).Take(pageFilter.PageSize.Value).ToArrayAsync();
            result.Customers = customers.Select(x => x.ToDto(false)).ToArray();

            return result;
        }
    }
}
