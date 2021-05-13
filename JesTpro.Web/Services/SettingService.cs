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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface ISettingService
    {
        Task<SettingDto[]> GetList(SettingFilterDto filter);
        Task<SettingDto> Get(int id);
        Task<SettingDto[]> Edit(SettingDto[] settings);
        Task<string> Upload(IFormFile file, int id);
        Task<SettingDto> GetByKey(string key);
    }
    public class SettingService : ISettingService { 
        MyDBContext _dbCtx;
        ILogger<SettingService> _logger;
        IOptions<AppSettings> _appSettings;
        public SettingService(MyDBContext dbCtx, ILogger<SettingService> logger, IOptions<AppSettings> appSettings)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<SettingDto[]> Edit(SettingDto[] settings)
        {
            foreach (var s in settings)
            {
                await EditSetting(s);
            }
            await _dbCtx.SaveChangesAsync();
            return settings;
        }

        private async Task EditSetting(SettingDto dto)
        {
            var ent = await _dbCtx.Settings.FindAsync(dto.Id);
            if (ent != null)
            {
                ent.Value = dto.Value;
                _dbCtx.Settings.Update(ent);
            }
        }

        public async Task<SettingDto[]> GetList(SettingFilterDto filter)
        {
            IQueryable<Setting> query = _dbCtx.Settings;

            if (filter.Categories != null && filter.Categories.Length > 0)
            {
                var res = await query.ToArrayAsync();
                var mainRes = new List<SettingDto>();
                foreach (var category in filter.Categories)
                {
                    //query = query.Where(x => x.Key.StartsWith(category));
                    mainRes.AddRange(res.Where(x => x.Key.StartsWith(category)).Select(x => x.ToDto()));
                }
                return mainRes.ToArray();
            }

            var result = await query.ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();
        }

        public async Task<SettingDto> Get(int id)
        {
            var res = await _dbCtx.Settings.FindAsync(id);
            return res.ToDto();
        }

        public async Task<SettingDto> GetByKey(string key)
        {
            var res = await _dbCtx.Settings.FirstOrDefaultAsync(x => x.Key.ToLower().Trim() == key.ToLower().Trim() );
            return res?.ToDto();
        }

        public async Task<string> Upload(IFormFile file, int id)
        {
            var setting = await _dbCtx.Settings.FindAsync(id);
            if (setting == null)
            {
                throw new System.Exception($"No setting fuond with id={id}");
            }

            var pathToSave = Path.Combine(_appSettings.Value.ImagePath, "settings", id.ToString());
            Directory.CreateDirectory(pathToSave);

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            setting.Value = fullPath;
            
            await _dbCtx.SaveChangesAsync();
            return fullPath;
        }
    }
}
