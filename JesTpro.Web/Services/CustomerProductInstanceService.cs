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
    public interface ICustomerProductInstanceService : ICRUDService<CustomerProductInstanceDto, CustomerProductInstanceFilterDto, CustomerProductInstanceEditDto>
    {
    }
    public class CustomerProductInstanceService : ICustomerProductInstanceService
    {
        MyDBContext _dbCtx;
        ILogger<CustomerProductInstanceService> _logger;
        IOptions<AppSettings> _appSettings;

        public CustomerProductInstanceService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<CustomerProductInstanceService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete CustomerProductInstance for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.CustomerProductInstances.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<CustomerProductInstanceDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get CustomerProductInstance for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<CustomerProductInstance> GetInner(Guid id)
        {
            return await _dbCtx.CustomerProductInstances.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomerProductInstanceDto[]> GetList(CustomerProductInstanceFilterDto filter)
        {
            _logger.LogDebug($"Calling getList CustomerProductInstance");

            IQueryable<CustomerProductInstance> query = _dbCtx.CustomerProductInstances;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            if (filter.IdCustomer != Guid.Empty)
            {
                query = query.Where(x => x.IdCustomer == filter.IdCustomer);
            }

            if (filter.IdReceipt == Guid.Empty)
            {
                query = query.Where(x => !x.IdReceipt.HasValue);
            }

            if (filter.NotExpired.HasValue)
            {
                if (filter.NotExpired.Value)
                {
                    query = query.Where(x => x.PaymentStatus == dal.Entities.PaymentStatus.Completed && (!x.ExpirationDate.HasValue || x.ExpirationDate.Value > DateTime.UtcNow));
                }
                else
                {
                    query = query.Where(x => x.PaymentStatus == dal.Entities.PaymentStatus.Completed && x.ExpirationDate.HasValue && x.ExpirationDate.Value <= DateTime.UtcNow);
                }
            }


            var result = await query.OrderBy(x => x.Name).ToArrayAsync();
            return result.Select(x => x.ToDto(false)).ToArray();

        }

        public async Task<CustomerProductInstanceDto> Save(CustomerProductInstanceEditDto itemToEdit)
        {
            CustomerProductInstance res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update CustomerProductInstance for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"CustomerProductInstance with id={itemToEdit.Id} not exists!");
                }
                res.CostAmount = itemToEdit.CostAmount;
                res.Description = itemToEdit.Description;
                res.Discount = itemToEdit.Discount;
                res.DiscountDescription = itemToEdit.DiscountDescription;
                res.ExpirationDate = itemToEdit.ExpirationDate;
                res.Price = itemToEdit.Price;
                res.IdReceipt = itemToEdit.IdReceipt;
                res.DiscountType = (int?)itemToEdit.DiscountType;
                
                 _dbCtx.CustomerProductInstances.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.PaymentStatus = dal.Entities.PaymentStatus.None;
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert CustomerProductInstance for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.CustomerProductInstances.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

    }
}
