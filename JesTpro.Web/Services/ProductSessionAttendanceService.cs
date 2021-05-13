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
    public interface IProductSessionAttendanceService : ICRUDService<ProductSessionAttendanceDto, ProductSessionAttendanceFilterDto, ProductSessionAttendanceEditDto>
    {
    }
    public class ProductSessionAttendanceService : IProductSessionAttendanceService
    {
        MyDBContext _dbCtx;
        ILogger<ProductSessionAttendanceService> _logger;
        IOptions<AppSettings> _appSettings;

        public ProductSessionAttendanceService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<ProductSessionAttendanceService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete ProductSessionAttendance for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.ProductSessionAttendances.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ProductSessionAttendanceDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get ProductSessionAttendance for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<ProductSessionAttendance> GetInner(Guid id)
        {
            return await _dbCtx.ProductSessionAttendances.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductSessionAttendanceDto[]> GetList(ProductSessionAttendanceFilterDto filter)
        {
            _logger.LogDebug($"Calling getList ProductSessionAttendance");

            IQueryable<ProductSessionAttendance> query = _dbCtx.ProductSessionAttendances;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.OrderByDescending(x => x.XCreateDate).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ProductSessionAttendanceDto> Save(ProductSessionAttendanceEditDto itemToEdit)
        {
            ProductSessionAttendance res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update ProductSessionAttendance for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"ProductSessionAttendance with id={itemToEdit.Id} not exists!");
                }
                res.CustomerFullName = itemToEdit.CustomerFullName;
                res.IdCustomer = itemToEdit.IdCustomer;
                res.IdSession = itemToEdit.IdSession;
                res.Present = itemToEdit.Present;
                 _dbCtx.ProductSessionAttendances.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert ProductSessionAttendance for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.ProductSessionAttendances.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

    }
}
