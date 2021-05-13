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
using System.Security.Claims;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IProductSessionService : ICRUDService<ProductSessionDto, ProductSessionFilterDto, ProductSessionEditDto>
    {
        Task<ProductSessionDto> SaveSubscribers(ProductSessionDto item);
    }
    public class ProductSessionService : IProductSessionService
    {
        MyDBContext _dbCtx;
        ILogger<ProductSessionService> _logger;
        IOptions<AppSettings> _appSettings;
        ClaimsPrincipal _claimPrincipal;
        ICustomerService _customerService;

        public ProductSessionService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ICustomerService customerService, ClaimsPrincipal claimPrincipal, ILogger<ProductSessionService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
            _claimPrincipal = claimPrincipal;
            _customerService = customerService;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete ProductSession for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.ProductSessions.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ProductSessionDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get ProductSession for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<ProductSession> GetInner(Guid id)
        {
            return await _dbCtx.ProductSessions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductSessionDto[]> GetList(ProductSessionFilterDto filter)
        {
            _logger.LogDebug($"Calling getList ProductSession");

            IQueryable<ProductSession> query = _dbCtx.ProductSessions;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }
            if (filter.IdProduct != Guid.Empty)
            {
                query = query.Where(x => x.IdProduct == filter.IdProduct);
            }

            var result = await query.OrderByDescending(x => x.XCreateDate).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ProductSessionDto> Save(ProductSessionEditDto itemToEdit)
        {
            ProductSession res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update ProductSession for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"ProductSession with id={itemToEdit.Id} not exists!");
                }
                res.Description = itemToEdit.Description;
                res.IdTeacher = itemToEdit.IdTeacher;
                res.IdProduct = itemToEdit.IdProduct;
                 _dbCtx.ProductSessions.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                var claims = ((ClaimsIdentity)_claimPrincipal.Identity).Claims;
                var userId = claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
                if (userId == null || string.IsNullOrEmpty(userId.Value)) {
                    throw new Exception("Cannot find teacherId");
                }
                res.IdTeacher = Guid.Parse(userId.Value);
                _logger.LogDebug($"Calling Insert ProductSession for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.ProductSessions.AddAsync(res);
                _dbCtx.SaveChanges();
                await AddSubscribers(res.IdProduct, res.Id);

            }
            return res.ToDto();
        }

        private async Task AddSubscribers(Guid idProduct, Guid idSession)
        {
            var customers = await _customerService.GetProductCustomers(new BasePaginatedFilterDto() { PageSize = 200 }, idProduct);
            foreach (var customer in customers.Customers)
            {
                var item = new ProductSessionAttendance();
                item.Id = Guid.NewGuid();
                item.IdSession = idSession;
                item.IdCustomer = customer.Id;
                item.CustomerFullName = $"{customer.FullName} {customer.FiscalCode}";
                _dbCtx.ProductSessionAttendances.Add(item);
            }
            await _dbCtx.SaveChangesAsync();
        }

        public async Task<ProductSessionDto> SaveSubscribers(ProductSessionDto item)
        {
            var subscribers = await _dbCtx.ProductSessionAttendances.Where(x => x.IdSession == item.Id).ToArrayAsync();
            foreach (var s in subscribers)
            {
                var newElemenet = item.ProductSessionAttendances.FirstOrDefault(x => x.Id == s.Id);
                if (newElemenet != null)
                {
                    s.Present = newElemenet.Present;
                }
            }
            _dbCtx.ProductSessionAttendances.UpdateRange(subscribers);
            await _dbCtx.SaveChangesAsync();
            var res = await _dbCtx.ProductSessions.FindAsync(item.Id);
            return res.ToDto();
        }
    }
}
