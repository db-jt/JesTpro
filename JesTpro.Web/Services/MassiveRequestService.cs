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

using ClosedXML.Excel;
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
    public interface IMassiveRequestService : ICRUDService<MassiveRequestDto, MassiveRequestFilterDto, MassiveRequestEditDto>
    {
        Task<bool> Upload(IFormFile file, Models.ImportType importType);
        Task<bool> CheckImports();
    }
    public class MassiveRequestService : IMassiveRequestService
    {
        MyDBContext _dbCtx;
        ILogger<MassiveRequestService> _logger;
        IOptions<AppSettings> _appSettings;
        private int _commitBlock = 50;

        public MassiveRequestService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<MassiveRequestService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete MassiveRequest for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.MassiveRequests.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<MassiveRequestDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get MassiveRequest for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<MassiveRequest> GetInner(Guid id)
        {
            return await _dbCtx.MassiveRequests.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MassiveRequestDto[]> GetList(MassiveRequestFilterDto filter)
        {
            _logger.LogDebug($"Calling getList MassiveRequest");

            IQueryable<MassiveRequest> query = _dbCtx.MassiveRequests;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.OrderByDescending(x => x.LastExecution.HasValue?DateTime.MaxValue:x.LastExecution.Value).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<MassiveRequestDto> Save(MassiveRequestEditDto itemToEdit)
        {
            MassiveRequest res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update MassiveRequest for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"MassiveRequest with id={itemToEdit.Id} not exists!");
                }
                res.Description = itemToEdit.Description;
                 _dbCtx.MassiveRequests.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                throw new NotImplementedException();

            }
            return res.ToDto();
        }

        public async Task<bool> Upload(IFormFile file, Models.ImportType importType)
        {
            
            var id = Guid.NewGuid();

            var pathToSave = Path.Combine(_appSettings.Value.MassiveImportPath, importType.ToString(), id.ToString()); //TODO: maybe savnig the user full name shuold be usefull but I wanto to keep away from strange chars..
            Directory.CreateDirectory(pathToSave);

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var item = new MassiveRequest();
            item.Description = $"Import '{fileName}'";
            item.FileToImport = fullPath;
            item.Id = id;
            item.ImportType = (dal.Entities.ImportType)importType;
            item.ImportStatus = dal.Entities.ImportStatus.New;
            await _dbCtx.MassiveRequests.AddAsync(item);
            await _dbCtx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckImports()
        {
            var item = await _dbCtx.MassiveRequests.Where(x => x.ImportStatus == dal.Entities.ImportStatus.New).OrderBy(x => x.XCreateDate).FirstOrDefaultAsync();
            if (item != null)
            {
                item.ImportStatus = dal.Entities.ImportStatus.Running;
                _dbCtx.MassiveRequests.Update(item);
                await _dbCtx.SaveChangesAsync();
                switch (item.ImportType)
                {
                    case dal.Entities.ImportType.Customer:
                        await ImportCustomers(item);
                        break;
                    default:
                        item.ImportStatus = dal.Entities.ImportStatus.Failed;
                        item.Error = $"Cannot find any import type with id = {item.ImportType}.";
                        item.LastExecution = DateTime.UtcNow;
                        _dbCtx.MassiveRequests.Update(item);
                        await _dbCtx.SaveChangesAsync();
                        break;
                }
            }
            return true;
        }

        private async Task ImportCustomers(MassiveRequest item)
        {
            if (!File.Exists(item.FileToImport))
            {
                item.ImportStatus = dal.Entities.ImportStatus.Failed;
                item.Error = $"Cannot find the specified file [{item.FileToImport}]";
                item.LastExecution = DateTime.UtcNow;
                _dbCtx.MassiveRequests.Update(item);
                await _dbCtx.SaveChangesAsync();
                return;
            }
            var errors = string.Empty;
            var deatils = string.Empty;
            CustomerType deafultCustomerType = null;
            var customerTypes = await _dbCtx.CustomerTypes.Where(x => !x.XDeleteDate.HasValue).ToArrayAsync();
            if (customerTypes.Length > 0)
            {
                deafultCustomerType = customerTypes[0];
            }
            var fileName = (new FileInfo(item.FileToImport)).Name;
            using (var excelWorkbook = new XLWorkbook(item.FileToImport))
            {
                var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed().Skip(1); // Skip header row
                var commitBlock = _commitBlock;
                var now = DateTime.UtcNow;
                foreach (var dataRow in nonEmptyDataRows)
                {
                    if (commitBlock < 0)
                    {
                        await _dbCtx.SaveChangesAsync();
                        commitBlock = _commitBlock;
                    }

                    var firstName = (string)dataRow.Cell("C").Value;
                    var lastName = (string)dataRow.Cell("B").Value;
                    var cf = (string) dataRow.Cell("G").Value;
                    var customer = await _dbCtx.Customers.FirstOrDefaultAsync(x => 
                        x.FiscalCode.ToLower() == cf.Trim().ToLower() 
                        && x.LastName.ToLower() == lastName.Trim().ToLower()
                        && x.FirstName.ToLower() == firstName.Trim().ToLower()
                    );
                    if (customer != null)
                    {
                        deatils += $"Skipping [{lastName} {firstName}] FiscalCode=[{cf}]: already present\n";
                        continue;
                    }
                    else
                    {
                        try
                        {
                            customer = new Customer();
                            customer.Id = Guid.NewGuid();
                            customer.LastName = (string)dataRow.Cell("B").Value;
                            customer.FirstName = (string)dataRow.Cell("C").Value;
                            customer.FullName = $"{customer.LastName} {customer.FirstName}";
                            customer.Gender = (string)dataRow.Cell("D").Value;
                            customer.BirthPlace = (string)dataRow.Cell("E").Value;
                            customer.BirthDate = (DateTime)dataRow.Cell("F").Value;
                            customer.FiscalCode = (string)dataRow.Cell("G").Value;
                            customer.Address = (string)dataRow.Cell("H") .Value;
                            customer.City = (string)dataRow.Cell("I").Value;
                            customer.PostalCode = (string)dataRow.Cell("J").Value;
                            customer.State = (string)dataRow.Cell("K").Value;
                            customer.Email = (string)dataRow.Cell("L").Value;
                            customer.PhoneNumber = (string)dataRow.Cell("M").Value;
                            if (!dataRow.Cell("N").IsEmpty())
                            {
                                customer.MembershipFeeExpiryDate = (DateTime)dataRow.Cell("N").Value;
                                customer.MembershipLastPayDate = now;
                                customer.IdType = deafultCustomerType == null ? Guid.Empty : deafultCustomerType.Id;
                                customer.Note = $"Imported from {fileName} at {now:dd/MM/yyyy HH:mm}";
                            }
                            await _dbCtx.Customers.AddAsync(customer);
                            commitBlock--;
                        } 
                        catch (Exception ex)
                        {
                            errors += $"Failed {cf}: {ex.Message}\n";
                        }
                    }
                   
                }
                item.ImportStatus = string.IsNullOrEmpty(errors) ? dal.Entities.ImportStatus.Completed : dal.Entities.ImportStatus.Failed;
                item.Description = string.IsNullOrEmpty(deatils) ? "OK" : deatils.TrimEnd();
                item.Error = errors.TrimEnd();
                item.LastExecution = DateTime.UtcNow;
                _dbCtx.MassiveRequests.Update(item);
                await _dbCtx.SaveChangesAsync();
            }
        }

    }
}
