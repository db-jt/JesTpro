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
using jt.jestpro.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface ITranslationService
    {
        Task<string> Get(string key);
    }
    public class TranslationService : ITranslationService { 
        MyDBContext _dbCtx;
        ILogger<TranslationService> _logger;
        IOptions<AppSettings> _appSettings;
        public TranslationService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<TranslationService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }
        public async Task<string> Get(string key)
        {
            var trans = await _dbCtx.Translates.FirstOrDefaultAsync(x => x.Key == key);
            var locale = _appSettings.Value.DefaultLocale.ToLower();
            if (locale == "it")
            {
                return trans.It;
            } 
            else if (locale == "fr")
            {
                return trans.Fr;
            }
            else if (locale == "es")
            {
                return trans.Es;
            }
            else if (locale == "de")
            {
                return trans.De;
            }
            else
            {
                return trans.En;
            }
        }
    }
}
