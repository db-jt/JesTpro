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

using jt.jestpro.Helpers;
using jt.jestpro.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace jt.jestpro
{
    public class AlarmWorker : BackgroundService
    {
        private readonly ILogger<AlarmWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AlarmWorker(IServiceScopeFactory serviceScopeFactory, ILogger<AlarmWorker> logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AlarmWorker: started");
            var forceCheck = false;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("Checking expirations");
                var appSettings = scope.ServiceProvider.GetService<IOptions<AppSettings>>();
                if (appSettings != null )
                {
                    forceCheck = (appSettings.Value != null ) && appSettings.Value.ForceExpirationCheckOnStart;
                }
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                int hourSpan = ((24 - DateTime.Now.Hour) + 6) % 24; //running approx at 6 AM ?

                if (hourSpan == 24 || forceCheck)
                {
                    forceCheck = false;
                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            _logger.LogInformation("Checking expirations");
                            var pubService = scope.ServiceProvider.GetRequiredService<IAlarmNotificationService>();
                            await pubService.CheckExpirations();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "AlarmWorker: unable to invoke alarm service");
                    }
                    hourSpan = 24;
                }
               
                await Task.Delay(TimeSpan.FromHours(hourSpan), stoppingToken);
            }
        }
    }
}
