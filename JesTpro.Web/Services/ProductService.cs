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
    public interface IProductService : ICRUDService<ProductDto, ProductFilterDto, ProductEditDto>
    {
    }
    public class ProductService : IProductService
    {
        MyDBContext _dbCtx;
        ILogger<ProductService> _logger;
        IOptions<AppSettings> _appSettings;

        public ProductService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<ProductService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete Product for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Products.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ProductDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get Product for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<Product> GetInner(Guid id)
        {
            return await _dbCtx.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductDto[]> GetList(ProductFilterDto filter)
        {
            _logger.LogDebug($"Calling getList Product");

            IQueryable<Product> query = _dbCtx.Products;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }
            if (filter.IdTeacher.HasValue && filter.IdTeacher != Guid.Empty)
            {
                query = query.Where(x => x.IdTeacher == filter.IdTeacher);
            }

            var result = await query.OrderBy(x => x.Name).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ProductDto> Save(ProductEditDto itemToEdit)
        {
            Product res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update Product for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"Product with id={itemToEdit.Id} not exists!");
                }
                res.Name = itemToEdit.Name;
                res.Description = itemToEdit.Description;
                res.CategoryName = itemToEdit.CategoryName;
                res.IdTeacher = itemToEdit.IdTeacher;
                res.EndDate = itemToEdit.EndDate;
                res.StartDate = itemToEdit.StartDate;
                 _dbCtx.Products.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert Product for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.Products.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

    }
}
