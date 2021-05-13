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

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Reflection;

namespace jt.jestpro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Init App");
                var fullName = typeof(Program).GetTypeInfo().Assembly.FullName;
                logger.Info($"Assembly: {fullName}");
                BuildWebHost(args).Run();
            }
            catch (System.Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
            //CreateHostBuilder(args).Build().Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((builderContext, config) =>
                   {
                       var env = builderContext.HostingEnvironment;

                       config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
                   })

        #region new Nlog
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   })
                   .UseNLog(new NLogAspNetCoreOptions() { CaptureMessageTemplates = true, CaptureMessageProperties = true, IncludeScopes = true }) // NLog: setup NLog for Dependency injection
        #endregion
                .ConfigureLogging(loggerFactory =>
                {

                    loggerFactory.AddDebug();
                    loggerFactory.AddNLog("nlog.config");

                })
                .UseContentRoot(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))
                .UseStartup<Startup>()
                .UseUrls("http://+:8080")
                .UseIISIntegration()

                .ConfigureServices(services => {
                    services.AddHostedService<BootstrapWorker>();
                    services.AddHostedService<ImportWorker>();
                    services.AddHostedService<AlarmWorker>();
                })

                .Build();


        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>()
        //                .UseUrls("http://localhost:5000");
        //        });
    }
}
