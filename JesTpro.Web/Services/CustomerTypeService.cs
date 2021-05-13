﻿// This file is part of JesTpro project.
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
    public interface ICustomerTypeService : ICRUDService<CustomerTypeDto, CustomerTypeFilterDto, CustomerTypeEditDto>
    {
    }
    public class CustomerTypeService : ICustomerTypeService
    {
        MyDBContext _dbCtx;
        ILogger<CustomerTypeService> _logger;
        IOptions<AppSettings> _appSettings;

        public CustomerTypeService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<CustomerTypeService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete CustomerType for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.CustomerTypes.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<CustomerTypeDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get CustomerType for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<CustomerType> GetInner(Guid id)
        {
            return await _dbCtx.CustomerTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomerTypeDto[]> GetList(CustomerTypeFilterDto filter)
        {
            _logger.LogDebug($"Calling getList CustomerType");

            IQueryable<CustomerType> query = _dbCtx.CustomerTypes;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.OrderBy(x => x.Name).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<CustomerTypeDto> Save(CustomerTypeEditDto dto)
        {
            CustomerType res;
            if (dto.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update CustomerType for id=[{dto.Id}]");
                //edit
                res = await this.GetInner(dto.Id);
                if (res == null)
                {
                    throw new NotFoundException($"CustomerType with id={dto.Id} not exists!");
                }

                res.CostAmount = dto.CostAmount;
                res.Description = dto.Description;
                res.Name = dto.Name;
                res.Duration = dto.Duration;

                 _dbCtx.CustomerTypes.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = dto.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert CustomerType for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.CustomerTypes.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

    }
}
