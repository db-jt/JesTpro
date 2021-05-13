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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.OpenApi.Models;
using jt.jestpro.dal;
using Microsoft.EntityFrameworkCore;
using jt.jestpro.Services;
using System.Collections.Generic;
using jt.jestpro.Helpers;
using Microsoft.Extensions.Logging;
using jt.jestpro.Mailer;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using jt.jestpro.Services.Contracts;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace jt.jestpro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddCors();
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimespanConverter());
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "jestpro JesTpro",
                    Description = "Test API with ASP.NET Core 3.0 for jestpro JesTpro",
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
               
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      
            var connString = Configuration.GetConnectionString("DefaultConnection");
            
            if (appSettings.UseSqLite)
            {
                // Local SQLite DB (not for production!!)
                DBCheckerSqlite.DoCheck(connString);
                services.AddDbContext<MyDBContext>(options =>
                    options.UseLazyLoadingProxies(true)
                    .UseSqlite(connString)
                    );
            }
            else
            {
                // MYSql/MariaDB standard DB
                DBChecker.DoCheck(connString);
                services.AddDbContext<MyDBContext>(options =>
                    options.UseLazyLoadingProxies(true)
                    .UseMySql(connString)
                    );
            }
            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductInstanceService, ProductInstanceService>();
            services.AddScoped<IProductSessionService, ProductSessionService>();
            services.AddScoped<IProductSessionAttendanceService, ProductSessionAttendanceService>();
            services.AddScoped<ICustomerProductInstanceService, CustomerProductInstanceService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerTypeService, CustomerTypeService>();
            services.AddScoped<IPaymentReceiptService, PaymentReceiptService>();
            services.AddScoped<ICreditNoteService, CreditNoteService>();
            services.AddScoped<IPaymentReceiptDetailService, PaymentReceiptDetailService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IMassiveRequestService, MassiveRequestService>();
            services.AddScoped<IAlarmNotificationService, AlarmNotificationService>();
            services.AddScoped<IBootstrapService, BootstrapService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IHtmlToPDF, DinkToPDF>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddTransient<ClaimsPrincipal>(s =>
                    s.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddTransient<ITemplateHelperService, TemplateHelperService>();
            services.AddTransient(typeof(Lazy<>), typeof(Lazy<>));
            services.AddScoped<ITranslationService, TranslationService>();

            services.AddRazorPages();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            if (appSettings.UseSqLite && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , ILogger<Startup> logger)
        {
            app.ConfigureExceptionHandler(logger);
            app.UseRouting();
           
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1 for jestpro JesTpro");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/app");
                });
            });

            app.Map("/app", builder => {
                builder.UseSpa(spa =>
                {
                    //if (env.IsDevelopment())
                    //{
                    //    spa.UseProxyToSpaDevelopmentServer($"http://localhost:4201/");
                    //}
                    //else
                    //{
                        var staticPath = Path.Combine(
                            Directory.GetCurrentDirectory(), $"app");
                        var fileOptions = new StaticFileOptions
                        { FileProvider = new PhysicalFileProvider(staticPath) };
                        builder.UseSpaStaticFiles(options: fileOptions);

                        spa.Options.DefaultPageStaticFileOptions = fileOptions;
                    //}
                });
            });

        }

    }
}
