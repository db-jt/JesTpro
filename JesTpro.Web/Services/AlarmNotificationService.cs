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
using jt.jestpro.Mailer;
using jt.jestpro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IAlarmNotificationService
    {
        Task<bool> SendExpirationMail(AlarmNotificationDto alarmNotification);
        Task CheckExpirations();
    }
    public class AlarmNotificationService : IAlarmNotificationService { 
        MyDBContext _dbCtx;
        ILogger<AlarmNotificationService> _logger;
        IOptions<AppSettings> _appSettings;
        private readonly Lazy<ITemplateHelperService> _templateService;
        private readonly IEmailService _mailService;
        private readonly ITranslationService _t;
        private readonly ISettingService _s;
        
        public AlarmNotificationService(MyDBContext dbCtx, Lazy<ITemplateHelperService> templateService, ISettingService s, IEmailService mailService, ITranslationService t, IOptions<AppSettings> appSettings, ILogger<AlarmNotificationService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
            _mailService = mailService;
            _t = t;
            _s = s;
            _templateService = templateService;
        }

        private async Task<bool> SendExpirationRemiderMail(AlarmNotificationDto alarmNotification)
        {
            var lang = _appSettings.Value.DefaultLocale;

            var mailMassage = new EmailMessage();
            var subject = "";
            mailMassage.ToAddresses.Add(new EmailAddress() { Name = alarmNotification.CustomerName, Address = alarmNotification.Email } );
            var template = "";
            switch (alarmNotification.AlarmNotificationType)
            {
                case AlarmNotificationType.Fee:
                    template = $"Emails/AlarmFee_{lang}.cshtml";
                    subject = await _t.Get("[email.alarm-fee]");
                    break;
                case AlarmNotificationType.MedicalCertificate:
                    template = $"Emails/AlarmCertificate_{lang}.cshtml";
                    subject = await _t.Get("[email.alarm-certificate]");
                    break;
                case AlarmNotificationType.Product:
                    template = $"Emails/AlarmProduct_{lang}.cshtml";
                    subject = await _t.Get("[email.alarm-product]");
                    break;
            }
            mailMassage.Subject = subject;
            mailMassage.Content = await _templateService.Value.GetTemplateHtmlAsStringAsync(template, alarmNotification);
            await _mailService.Send(mailMassage);
            return true;
        }

        public async Task CheckExpirations()
        {
            var feeAndCertPrev = 1;
            var sMonth = await _s.GetByKey("company.expiryMonthLimit");
            if (sMonth != null)
            {
                int.TryParse(sMonth.Value, out feeAndCertPrev);
            }

            var productPrev = 7;
            var sProduct = await _s.GetByKey("company.expiryProductDaysLimit");
            if (sProduct != null)
            {
                int.TryParse(sProduct.Value, out productPrev);
            }


            var checkDate = DateTime.UtcNow.AddMonths(feeAndCertPrev);
            TimeSpan ts = new TimeSpan(0, 0, 0);
            var beginDate = checkDate.Date + ts;
            TimeSpan ts2 = new TimeSpan(23, 59, 59);
            var endDate = checkDate.Date + ts2;

            //FEES
            var customers = await _dbCtx.Customers.Where(x => !x.XDeleteDate.HasValue && (x.MembershipFeeExpiryDate.HasValue && x.MembershipFeeExpiryDate.Value >= beginDate && x.MembershipFeeExpiryDate.Value <= endDate)).ToArrayAsync();
            foreach (var customer in customers)
            {
                var alarmNotification = new AlarmNotificationDto()
                {
                    AlarmNotificationType = AlarmNotificationType.Fee,
                    CustomerName = customer.FullName,
                    ExpirationDate = customer.MembershipFeeExpiryDate,
                    Email = customer.Email
                };
                try
                {
                    _logger.LogInformation($"Sending feeExpiration alarm to {customer.Email}: expire: {customer.MembershipFeeExpiryDate}");
                    await SendExpirationRemiderMail(alarmNotification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to send feeExpiration alarm to ${customer.Email}");
                }
            }

            //CERTIFICATES
            customers = await _dbCtx.Customers.Where(x => !x.XDeleteDate.HasValue && (x.MedicalCertificateExpiration.HasValue && x.MedicalCertificateExpiration.Value >= beginDate && x.MedicalCertificateExpiration.Value <= endDate)).ToArrayAsync();
            foreach (var customer in customers)
            {
                var alarmNotification = new AlarmNotificationDto()
                {
                    AlarmNotificationType = AlarmNotificationType.MedicalCertificate,
                    CustomerName = customer.FullName,
                    ExpirationDate = customer.MedicalCertificateExpiration,
                    Email = customer.Email
                };
                try
                {
                    _logger.LogInformation($"Sending certificateExpiration alarm to {customer.Email}: expire: {customer.MembershipFeeExpiryDate}");
                    await SendExpirationRemiderMail(alarmNotification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to send certtificateExpiration alarm to {customer.Email}");
                }
            }

            var checkDateProduct = DateTime.UtcNow.AddDays(productPrev);
            var beginDateProduct = checkDateProduct.Date + ts;
            var endDateProduct = checkDateProduct.Date + ts2;

            //PRODUCTS
            var products = await _dbCtx.CustomerProductInstances.Where(x => !x.XDeleteDate.HasValue && (x.ExpirationDate.HasValue && x.ExpirationDate.Value >= beginDateProduct && x.ExpirationDate.Value <= endDateProduct)).ToArrayAsync();
            foreach (var product in products)
            {
                var alarmNotification = new AlarmNotificationDto()
                {
                    AlarmNotificationType = AlarmNotificationType.Product,
                    CustomerName = product.Customer.FullName,
                    Description = $"{product.Name}\n{product.Description}",
                    ExpirationDate = product.ExpirationDate,
                    Email = product.Customer.Email
                };
                try
                {
                    _logger.LogInformation($"Sending product {product.Name} id='{product.Id}' expiration alarm to ${product.Customer.Email}: expire: ${product.ExpirationDate}");
                    await SendExpirationRemiderMail(alarmNotification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to send product alarm to ${product.Customer.Email}");
                }
            }
        }

        public async Task<bool> SendExpirationMail(AlarmNotificationDto alarmNotification)
        {
            if (!alarmNotification.IdCustomer.HasValue)
            {
                throw new Exception("Missing customer, unable to send notification");
            }
            var customer = await _dbCtx.Customers.FindAsync(alarmNotification.IdCustomer);
            if (customer == null)
            {
                throw new Exception("Customer cannot be loaded, unable to send notification");
            }
            if (alarmNotification.AlarmNotificationType == AlarmNotificationType.Product)
            {
                if (!alarmNotification.IdCustomerProductInstance.HasValue)
                {
                    throw new Exception("Missing product, unable to send notification");
                }
                var product = await _dbCtx.CustomerProductInstances.FindAsync(alarmNotification.IdCustomerProductInstance);
                if (product == null)
                {
                    throw new Exception("Customer product cannot be loaded, unable to send notification");
                }

                alarmNotification.CustomerName = product.Customer.FullName;
                alarmNotification.Description = $"{product.Name}\n{product.Description}";
                alarmNotification.ExpirationDate = product.ExpirationDate;
                alarmNotification.Email = product.Customer.Email;
                _logger.LogDebug($"Sending product {product.Name} id='{product.Id}' expiration alarm to ${product.Customer.Email}: expire: ${product.ExpirationDate}");
                await SendExpirationRemiderMail(alarmNotification);
            } 
            else //FEE AND MEDICAL CERT
            {
                alarmNotification.CustomerName = customer.FullName;
                alarmNotification.ExpirationDate = alarmNotification.AlarmNotificationType == AlarmNotificationType.MedicalCertificate ? customer.MedicalCertificateExpiration : customer.MembershipFeeExpiryDate;
                alarmNotification.Email = customer.Email;
                _logger.LogDebug($"Sending {alarmNotification.AlarmNotificationType} alarm to {customer.Email}: expire: {customer.MembershipFeeExpiryDate}");
                await SendExpirationRemiderMail(alarmNotification);
            }
            return true;
        }
    }
}
