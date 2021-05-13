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
    public interface IProductInstanceService : ICRUDService<ProductInstanceDto, ProductInstanceFilterDto, ProductInstanceEditDto>
    {
    }
    public class ProductInstanceService : IProductInstanceService
    {
        MyDBContext _dbCtx;
        ILogger<ProductInstanceService> _logger;
        IOptions<AppSettings> _appSettings;

        public ProductInstanceService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<ProductInstanceService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete ProductInstance for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.ProductInstances.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ProductInstanceDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get ProductInstance for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<ProductInstance> GetInner(Guid id)
        {
            return await _dbCtx.ProductInstances.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductInstanceDto[]> GetList(ProductInstanceFilterDto filter)
        {
            _logger.LogDebug($"Calling getList ProductInstance");

            IQueryable<ProductInstance> query = _dbCtx.ProductInstances;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.OrderBy(x => x.Name).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ProductInstanceDto> Save(ProductInstanceEditDto itemToEdit)
        {
            ProductInstance res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update ProductInstance for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"ProductInstance with id={itemToEdit.Id} not exists!");
                }
                res.Description = itemToEdit.Description;
                res.Name = itemToEdit.Name;
                res.IdProduct = itemToEdit.IdProduct;
                res.Months = itemToEdit.Months;
                res.Price = itemToEdit.Price;
                res.Weeks = itemToEdit.Weeks;
                res.Years = itemToEdit.Years;
                res.Days = itemToEdit.Days;

                _dbCtx.ProductInstances.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert ProductInstance for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.ProductInstances.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

    }
}
