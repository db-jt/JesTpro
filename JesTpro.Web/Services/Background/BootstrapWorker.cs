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
    public class BootstrapWorker : BackgroundService
    {
        private readonly ILogger<BootstrapWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BootstrapWorker(IServiceScopeFactory serviceScopeFactory, ILogger<BootstrapWorker> logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("\n\n" +
                        "_______- *** WELCOME TO *** -_______" +
                "\n" + @"   ___          _____               " +
                "\n" + @"  |_  |        |_   _|              " +
                "\n" + @"    | | ___  ___ | |_ __  _ __ ___  " +
                "\n" + @"    | |/ _ \/ __|| | '_ \| '__/ _ \ " +
                "\n" + @"/\__/ /  __/\__ \| | |_) | | | (_) |" +
                "\n" + @"\____/ \___||___/\_/ .__/|_|  \___/ " +
                "\n" + @"                   | |              " +
                "\n" + @"                   |_|              " +
                "\n" +  "____________________________________" +
                "\n");
            
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _logger.LogInformation("MySqlWorker: ready to invoke service");
                    var pubService = scope.ServiceProvider.GetRequiredService<IBootstrapService>();
                    await pubService.CheckAdminUser();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"MySqlWorker: unable to invoke MySqlWorker service");
            }
           
        }
    }
}
