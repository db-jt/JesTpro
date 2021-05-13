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

using jt.jestpro.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace jt.jestpro
{
    public class ImportWorker : BackgroundService
    {
        private readonly ILogger<ImportWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private int count;

        public ImportWorker(IServiceScopeFactory serviceScopeFactory, ILogger<ImportWorker> logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            count = 0;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                if (count % 100 == 0)
                {
                    _logger.LogInformation("ImportWorker is still running");
                    count = 1;
                } 
                else
                {
                    count++;
                }
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var pubService = scope.ServiceProvider.GetRequiredService<IMassiveRequestService>();
                        await pubService.CheckImports();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ImportWorked, fail to invoke requests!");
                }
                finally
                {
                    await Task.Delay(30000, stoppingToken);
                }
            }

        }
    }
}
