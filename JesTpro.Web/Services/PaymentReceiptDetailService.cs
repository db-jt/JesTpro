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
    public interface IPaymentReceiptDetailService : ICRUDService<PaymentReceiptDetailDto, PaymentReceiptDetailFilterDto, PaymentReceiptDetailEditDto>
    {
    }
    public class PaymentReceiptDetailService : IPaymentReceiptDetailService
    {
        MyDBContext _dbCtx;
        ILogger<PaymentReceiptDetailService> _logger;
        IOptions<AppSettings> _appSettings;

        public PaymentReceiptDetailService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<PaymentReceiptDetailService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete PaymentReceiptDetail for id=[{id}]");
            var t = await this.GetInner(id);
            await removePaymentResource(t.ReceiptDetailType, t.IdResource, false);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.PaymentReceiptDetails.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<PaymentReceiptDetailDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get PaymentReceiptDetail for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<PaymentReceiptDetail> GetInner(Guid id)
        {
            return await _dbCtx.PaymentReceiptDetails.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaymentReceiptDetailDto[]> GetList(PaymentReceiptDetailFilterDto filter)
        {
            _logger.LogDebug($"Calling getList PaymentReceiptDetail");

            IQueryable<PaymentReceiptDetail> query = _dbCtx.PaymentReceiptDetails;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }
           
            var result = await query.OrderByDescending(x => x.XUpdateDate).ThenBy(x => x.XCreateDate).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<PaymentReceiptDetailDto> Save(PaymentReceiptDetailEditDto itemToEdit)
        {
            PaymentReceiptDetail res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update PaymentReceiptDetail for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"PaymentReceiptDetail with id={itemToEdit.Id} not exists!");
                }
                await removePaymentResource(res.ReceiptDetailType, res.IdResource, false);
                res.CostAmount = itemToEdit.CostAmount * itemToEdit.ProductAmount;
                res.Name = itemToEdit.Name;
                res.Description = itemToEdit.Description;
                res.IdResource = itemToEdit.IdResource;
                res.ProductAmount = itemToEdit.ProductAmount;
                res.ReceiptDetailType = (dal.Entities.ReceiptDetailType)itemToEdit.ReceiptDetailType;
                 _dbCtx.PaymentReceiptDetails.Update(res);
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert PaymentReceiptDetail for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.PaymentReceiptDetails.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            // add receipt reference to destination table
            await addPaymentResource(itemToEdit.ReceiptDetailType, itemToEdit.IdResource, itemToEdit.IdReceipt, false);
            _dbCtx.SaveChanges();
            return res.ToDto();
        }

        private async Task<bool> removePaymentResource(dal.Entities.ReceiptDetailType receiptDetailType, Guid idResource, bool saveChanges = true)
        {
            switch (receiptDetailType)
            {
                case dal.Entities.ReceiptDetailType.Fee:
                    //something to do? maybe
                    break;
                case dal.Entities.ReceiptDetailType.Other:
                    //something to do? maybe
                    break;
                case dal.Entities.ReceiptDetailType.Product:
                    var item = _dbCtx.CustomerProductInstances.Find(idResource);
                    if (item != null)
                    {
                        //item.IdReceipt = null;
                        //item.PaymentStatus = dal.Entities.PaymentStatus.None;
                        _dbCtx.CustomerProductInstances.Remove(item);
                        if (saveChanges)
                        {
                            await _dbCtx.SaveChangesAsync();
                        }
                    }
                    break;
            }
            return true;
        }

        private async Task<bool> addPaymentResource(Models.ReceiptDetailType receiptDetailType, Guid idResource, Guid idReceipt, bool saveChanges = true)
        {
            switch (receiptDetailType)
            {
                case Models.ReceiptDetailType.Fee:
                    //something to do? maybe
                    break;
                case Models.ReceiptDetailType.Other:
                    //something to do? maybe
                    break;
                case Models.ReceiptDetailType.Product:
                    var item = _dbCtx.CustomerProductInstances.Find(idResource);
                    if (item != null)
                    {
                        item.IdReceipt = idReceipt;
                        item.PaymentStatus = dal.Entities.PaymentStatus.WaitingForPayment;
                        _dbCtx.CustomerProductInstances.Update(item);
                        if (saveChanges)
                            await _dbCtx.SaveChangesAsync();
                    }
                    break;
            }
            return true;
        }

    }
}
