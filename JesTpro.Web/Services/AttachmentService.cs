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
    public interface IAttachmentService : ICRUDService<AttachmentDto, AttachmentFilterDto, AttachmentEditDto>
    {
        Task<bool> Upload(IFormFile file, Guid id);
        Task<string> GetFullPath(Guid idResource);
    }
    public class AttachmentService : IAttachmentService
    {
        MyDBContext _dbCtx;
        ILogger<AttachmentService> _logger;
        IOptions<AppSettings> _appSettings;

        public AttachmentService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<AttachmentService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete Attachment for id=[{id}]");
            var t = await this.GetInner(id);
            if (t!= null && System.IO.File.Exists(t.FullPath))
            {
                var directory = Path.GetDirectoryName(t.FullPath);
                Directory.Delete(directory, true);
                //File.Delete(t.FullPath);
            }
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Attachments.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<AttachmentDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get Attachment for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<Attachment> GetInner(Guid id)
        {
            return await _dbCtx.Attachments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AttachmentDto[]> GetList(AttachmentFilterDto filter)
        {
            var idResource = filter.IdResource;
            _logger.LogDebug($"Calling getList Attachment");
            if (idResource == Guid.Empty)
            {
                throw new Exception("Missing IdResource, unable to save attachment");
            }

            IQueryable<Attachment> query = _dbCtx.Attachments;
            query = query.Where(x => x.IdResource == idResource);

            var result = await query.OrderBy(x => x.Name).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public Task<AttachmentDto> Save(AttachmentEditDto filter)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Upload(IFormFile file, Guid idResource)
        {
            if (idResource == Guid.Empty)
            {
                throw new Exception("Missing IdResource, unable to save attachment");
            }

            var id = Guid.NewGuid();

            var pathToSave = Path.Combine(_appSettings.Value.AttachmentPath, idResource.ToString(), id.ToString()); //TODO: maybe savnig the user full name shuold be usefull but I wanto to keep away from strange chars..
            Directory.CreateDirectory(pathToSave);

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var item = new Attachment();
            item.Name = fileName;
            item.FullPath = fullPath;
            item.Id = id;
            item.IdResource = idResource;
            item.Size = new System.IO.FileInfo(fullPath).Length;
            await _dbCtx.Attachments.AddAsync(item);
            await _dbCtx.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetFullPath(Guid id)
        {
            var item = await GetInner(id);
            if (item == null)
            {
                throw new System.Exception($"No attachment fuond with id={id}");
            }
            if (string.IsNullOrWhiteSpace(item.FullPath))
            {
                throw new Exception($"No path found for customer {item.FullPath}");
            }
            if (!System.IO.File.Exists(item.FullPath))
            {
                throw new Exception($"The attachement {item.FullPath} is no longer available");
            }
            return item.FullPath;
        }
    }
}
