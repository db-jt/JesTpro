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
using jt.jestpro.Mailer;
using jt.jestpro.Models;
using jt.jestpro.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IPaymentReceiptService : ICRUDService<PaymentReceiptDto, PaymentReceiptFilterDto, PaymentReceiptEditDto>
    {
        Task<PaymentReceiptDto> GenerateInvoice(InvoiceRequestData requestData);
        Task<PaymentReceiptDto> CreateFastInvoice(PaymentReceiptEditForFastInvoceDto requestData);
        Task<bool> GeneratePdfInvoice(Guid invoiceId);
        Task<string> GetPdfInvoicePath(Guid invoiceId);
        Task<string> SendPdfInvoiceViaMail(Guid invoiceId, string emailEddress = null);


    }
    public class PaymentReceiptService : IPaymentReceiptService
    {
        MyDBContext _dbCtx;
        ILogger<PaymentReceiptService> _logger;
        IOptions<AppSettings> _appSettings;
        private readonly Lazy<ITemplateHelperService> _templateService;
        private readonly Lazy<IPaymentReceiptDetailService> _receiptDetailService;
        private readonly Lazy<ICustomerProductInstanceService> _customerInstanceProductService;
        private readonly Lazy<IHtmlToPDF> _pdfService;
        private readonly IEmailService _mailService;
        private readonly ITranslationService _t;
        private readonly Lazy<ClaimsPrincipal> _claimPrincipal;

        public PaymentReceiptService(MyDBContext dbCtx, Lazy<ITemplateHelperService> templateService, Lazy<IHtmlToPDF> pdfService, IEmailService mailService, Lazy<ClaimsPrincipal> claimPrincipal, Lazy<IPaymentReceiptDetailService> receiptDetailService, Lazy<ICustomerProductInstanceService> customerInstanceProductService, ITranslationService t, IOptions<AppSettings> appSettings, ILogger<PaymentReceiptService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _templateService = templateService;
            _pdfService = pdfService;
            _appSettings = appSettings;
            _mailService = mailService;
            _receiptDetailService = receiptDetailService;
            _claimPrincipal = claimPrincipal;
            _t = t;
            _customerInstanceProductService = customerInstanceProductService;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete PaymentReceipt for id=[{id}]");
            var now = DateTime.UtcNow;
            var t = await this.GetInner(id);
            foreach (var det in t.PaymentReceiptDetails.Where(x => x.ReceiptDetailType == dal.Entities.ReceiptDetailType.Product))
            {
                var p = await _dbCtx.CustomerProductInstances.FindAsync(det.IdResource);
                if (p != null)
                {
                    p.PaymentStatus = dal.Entities.PaymentStatus.Aborted;
                    p.XDeleteDate = now;
                    _dbCtx.CustomerProductInstances.Update(p);
                    await _dbCtx.SaveChangesAsync();
                }
            }
            t.XDeleteDate = now;
            _dbCtx.PaymentReceipts.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<PaymentReceiptDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get PaymentReceipt for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<PaymentReceipt> GetInner(Guid id)
        {
            return await _dbCtx.PaymentReceipts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaymentReceiptDto[]> GetList(PaymentReceiptFilterDto filter)
        {
            _logger.LogDebug($"Calling getList PaymentReceipt");
            if (!filter.StartDate.HasValue)
            {
                var now = DateTime.UtcNow;
                filter.StartDate = new DateTime(now.Year, 1, 1);
                filter.StartDate = Utils.GetDateForStart(filter.StartDate.Value);
            }
            if (!filter.EndDate.HasValue)
            {
                filter.EndDate = DateTime.MaxValue;
                filter.EndDate = Utils.GetDateForEnd(filter.EndDate.Value);
            }

            IQueryable<PaymentReceipt> query = _dbCtx.PaymentReceipts;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            query = query.Where(x => (x.IssueDate >= filter.StartDate.Value && x.IssueDate <= filter.EndDate.Value));

            if (filter.IdCustomer.HasValue && filter.IdCustomer.Value != Guid.Empty)
            {
                query = query.Where(x => x.IdCustomer == filter.IdCustomer.Value);
            }
            if (filter.WorkableOnly.HasValue && filter.WorkableOnly.Value)
            {
                query = query.Where(x => x.InvoiceNumber == null);
            }
            

            var result = await query.OrderByDescending(x => x.PaymentDate.HasValue ? x.PaymentDate.Value : DateTime.MaxValue ).ThenByDescending(x => x.IssueDate).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<PaymentReceiptDto> Save(PaymentReceiptEditDto itemToEdit)
        {
            PaymentReceipt res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update PaymentReceipt for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"PaymentReceipt with id={itemToEdit.Id} not exists!");
                }
                //res.CostAmount = itemToEdit.CostAmount;
                res.PaymentDate = itemToEdit.PaymentDate;
                res.Name = itemToEdit.Name;
                res.Description = itemToEdit.Description;
                _dbCtx.PaymentReceipts.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert PaymentReceipt for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.PaymentReceipts.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

        public async Task<PaymentReceiptDto> GenerateInvoice(InvoiceRequestData invoiceData)
        {
            var receipt = await GetInner(invoiceData.IdReceipt);

            if (receipt == null)
            {
                throw new Exception($"No receipt found with id = {invoiceData.IdReceipt}");
            }

            //Discount management
            if (invoiceData.Discount.HasValue)
            {
                decimal discountValue = 0;
                if (invoiceData.DiscountType == DiscountType.Percentage)
                {
                    var subTotal = receipt.PaymentReceiptDetails.Sum(x => x.CostAmount);
                    discountValue = subTotal / 100 * invoiceData.Discount.Value;
                } 
                else
                {
                    discountValue = invoiceData.Discount.Value;
                }

                await _receiptDetailService.Value.Save(new PaymentReceiptDetailEditDto()
                {
                    CostAmount = discountValue * (-1),
                    ReceiptDetailType = Models.ReceiptDetailType.Discount,
                    IdReceipt = invoiceData.IdReceipt,
                    Description = invoiceData.DiscountDescription,
                    ProductAmount = 1,
                    Name = $"{invoiceData.Discount.Value} " + (invoiceData.DiscountType == DiscountType.Percentage ? "%" : "€")
                });

                receipt = await GetInner(invoiceData.IdReceipt);
            }

            if (!invoiceData.Year.HasValue)
            {
                invoiceData.Year = DateTime.UtcNow.Year;
            }

            if (_appSettings.Value.UseSqLite)
            {
                await CreateInvoice(invoiceData, receipt);
            }
            else
            {
                await InvokeInvoiceSP(invoiceData, receipt);
            }

            #region UPDATE FEES
            var now = DateTime.UtcNow;
            var customer = _dbCtx.Customers.Find(receipt.IdCustomer);
            if (customer != null)
            {
                var fees = receipt.PaymentReceiptDetails.Where(x => x.ReceiptDetailType == dal.Entities.ReceiptDetailType.Fee);
                foreach (var fee in fees)
                {
                    
                    if (customer.MembershipFeeExpiryDate.HasValue)
                    {
                        customer.MembershipFeeExpiryDate = customer.MembershipFeeExpiryDate.Value.AddYears(1);
                    } 
                    else
                    {
                        customer.MembershipFeeExpiryDate = now.AddYears(1);
                    }
                    customer.MembershipLastPayDate = now;
                    _dbCtx.Customers.Update(customer);
                    
                }
                await _dbCtx.SaveChangesAsync();
            }

            #endregion

            #region UPDATE PRODUCTS
            var products = receipt.PaymentReceiptDetails.Where(x => x.ReceiptDetailType == dal.Entities.ReceiptDetailType.Product).ToArray();
            foreach (var product in products)
            {
                var p = await _dbCtx.CustomerProductInstances.FindAsync(product.IdResource);
                if (p != null)
                {
                    p.PaymentStatus = dal.Entities.PaymentStatus.Completed;
                    _dbCtx.CustomerProductInstances.Update(p);
                    await _dbCtx.SaveChangesAsync();
                }
            }
            #endregion

            #region UPDATE PAYMENT REVERSAL
            var reversals = receipt.PaymentReceiptDetails.Where(x => x.ReceiptDetailType == dal.Entities.ReceiptDetailType.PaymentReversal).ToArray();
            foreach (var reversal in reversals)
            {
                var customerProductInstance = await _dbCtx.CustomerProductInstances.FindAsync(reversal.IdResource);
                if (customerProductInstance == null)
                {
                    _logger.LogWarning($"No customer product found for reversal ReceiptPaymentDetail={reversal.Id} and customerProductIstance={reversal.IdResource}");
                }
                else
                {
                    customerProductInstance.IdReversal = reversal.Id;
                    customerProductInstance.ReversalDate = now;
                    customerProductInstance.ReversalCredit = reversal.CostAmount;
                    customerProductInstance.ReversalDescription = reversal.Description;
                    customerProductInstance.ExpirationDate = now;
                    _dbCtx.CustomerProductInstances.Update(customerProductInstance);
                    await _dbCtx.SaveChangesAsync();
                }
            }


            #endregion

            return await Get(invoiceData.IdReceipt);
        }

        public async Task<bool> GeneratePdfInvoice(Guid invoiceId)
        {
            var model = await this.GetInner(invoiceId);
            var user = _dbCtx.Users.Find(new Guid(_claimPrincipal.Value.Identity.Name));
            var lang = string.IsNullOrWhiteSpace(user.Lang) ? _appSettings.Value.DefaultLocale : user.Lang.ToUpper();
            var viewModel = new InvoiceDto();
            viewModel.Id = model.Id;
            viewModel.PaymentReceipt = model.ToDto();
            viewModel.Settings = await _dbCtx.Settings.Where(x => x.Key.ToLower().StartsWith("company")).Select(x => x.ToDto()).ToArrayAsync();
            var html = await _templateService.Value.GetTemplateHtmlAsStringAsync($"Invoice/Invoice_{lang}.cshtml", viewModel);
            //var html = await _razorViewToStringRenderer.Value.RenderViewToStringAsync($"/Views/Templates/Invoice/Invoice_{_appSettings.Value.DefaultLocale}.cshtml", model.ToDto());
            var receiptPath = await _pdfService.Value.CreateReport(html, model.InvoiceNumber, _appSettings.Value.PdfSettings);
            model = await this.GetInner(invoiceId);
            model.ReceiptPath = receiptPath.Replace("\\","/");
            _dbCtx.PaymentReceipts.Update(model);
            await _dbCtx.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetPdfInvoicePath(Guid invoiceId)
        {
            var invoice = await this.GetInner(invoiceId);
            if (string.IsNullOrWhiteSpace(invoice.ReceiptPath) || !File.Exists(invoice.ReceiptPath))
            {
                await GeneratePdfInvoice(invoiceId);
                invoice = await this.GetInner(invoiceId);
            }

            if (string.IsNullOrWhiteSpace(invoice.ReceiptPath))
            {
                _logger.LogError($"No pdf found for invoiceId=[{invoiceId}]");
                throw new Exception("No pdf found for this invoiceId");
            } 
            else
            {
                return invoice.ReceiptPath;
            }

        }

        public async Task<string> SendPdfInvoiceViaMail(Guid invoiceId, string emailEddress = "")
        {
            var invoice = await GetInner(invoiceId);
            if (string.IsNullOrEmpty(invoice.ReceiptPath) || !File.Exists(invoice.ReceiptPath))
            {
                await GeneratePdfInvoice(invoiceId);
            }
            
            var mailMassage = new EmailMessage();
            mailMassage.Subject = await _t.Get("[email.invoice]");
            var toAddr = string.IsNullOrWhiteSpace(emailEddress) ? invoice.Customer.Email : emailEddress;
            mailMassage.ToAddresses.Add(string.IsNullOrWhiteSpace(emailEddress) ? new EmailAddress() { Name = $"{invoice.Customer.FirstName} {invoice.Customer.LastName}",  Address = invoice.Customer.Email } : new EmailAddress() {Name = emailEddress, Address = emailEddress });
            if (!string.IsNullOrWhiteSpace(invoice.Customer.TutorEmail) && !toAddr.ToLower().Equals(invoice.Customer.TutorEmail.ToLower())) {
                mailMassage.ToAddresses.Add(new EmailAddress() { Name = $"{invoice.Customer.TutorFirstName} {invoice.Customer.TutorLastName}", Address = invoice.Customer.TutorEmail });
                toAddr = $"{toAddr}, {invoice.Customer.TutorEmail}";
            }
            var user = _dbCtx.Users.Find(new Guid(_claimPrincipal.Value.Identity.Name));
            var lang = string.IsNullOrWhiteSpace(user.Lang) ? _appSettings.Value.DefaultLocale : user.Lang.ToUpper();
            mailMassage.Content = await _templateService.Value.GetTemplateHtmlAsStringAsync($"Emails/Invoice_{lang}.cshtml", invoice.ToDto());
            mailMassage.Attachments.Add(invoice.ReceiptPath);
            await _mailService.Send(mailMassage);
            return toAddr;
        }

        public async Task<PaymentReceiptDto> CreateFastInvoice(PaymentReceiptEditForFastInvoceDto data)
        {
            var editRecipt = new PaymentReceiptEditDto()
            {
                CostAmount = data.CostAmount,
                Description = data.Description,
                IdCustomer = data.IdCustomer,
                IssueDate = data.IssueDate,
                PaymentType = data.PaymentType,
                Name = data.Name
            };
            var savedReceipt = await Save(editRecipt);
            var detailList = new List<PaymentReceiptDetailDto>();
            foreach (var d in data.PaymentReceiptDetails) 
            {
                var customerInstance = new CustomerProductInstanceDto();
                if (d.CustomerProductInstance != null) {
                    var editCustomerProduct = new CustomerProductInstanceEditDto()
                    {
                        CostAmount = d.CustomerProductInstance.CostAmount,
                        Description = d.CustomerProductInstance.Description,
                        Discount = d.CustomerProductInstance.Discount,
                        DiscountDescription = d.CustomerProductInstance.DiscountDescription,
                        DiscountType = d.CustomerProductInstance.DiscountType,
                        ExpirationDate = d.CustomerProductInstance.ExpirationDate,
                        IdCustomer = d.CustomerProductInstance.IdCustomer,
                        IdProductInstance = d.CustomerProductInstance.IdProductInstance,
                        IdReceipt = savedReceipt.Id,
                        Name = d.CustomerProductInstance.Name,
                        Price = d.CustomerProductInstance.Price,
                        ReversalCredit = d.CustomerProductInstance.ReversalCredit,
                        ReversalDate = d.CustomerProductInstance.ReversalDate,
                        IdReversal = d.CustomerProductInstance.IdReversal
                    };
                    customerInstance = await _customerInstanceProductService.Value.Save(editCustomerProduct);
                }

                var editReceiptDetail = new PaymentReceiptDetailEditDto() {
                    CostAmount = d.CostAmount,
                    Description = d.Description,
                    IdReceipt = savedReceipt.Id,
                    IdResource = customerInstance.Id != null && customerInstance.Id != Guid.Empty ? customerInstance.Id : d.IdResource,
                    Name = d.Name,
                    ProductAmount = d.ProductAmount,
                    ReceiptDetailType = d.ReceiptDetailType
                };

                await _receiptDetailService.Value.Save(editReceiptDetail);
            }

            var invoiceData = new InvoiceRequestData()
            {
                CustomerAddress = data.CustomerAddress,
                CustomerFiscalCode = data.CustomerFiscalCode,
                CustomerName = data.CustomerName,
                IdReceipt = savedReceipt.Id,
                Description = data.Description,
                Discount = data.Discount,
                DiscountDescription = data.DiscountDescription,
                DiscountType = data.DiscountType
            };

            return await GenerateInvoice(invoiceData);
        }

        private async Task CreateInvoice(InvoiceRequestData invoiceData, PaymentReceipt receipt)
        {
            // SPECIFIC FOR SQLite DB
            var trx = await _dbCtx.Database.BeginTransactionAsync();
            await _dbCtx.Database.OpenConnectionAsync();
            var connection = _dbCtx.Database.GetDbConnection();

            try
            {

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT COALESCE(MAX(number),0)
                                    FROM `invoice_number` 
                                    WHERE `year` = $selectedYear ";

                command.Parameters.Add(new SqliteParameter("$selectedYear", invoiceData.Year));
                var invoiceNumber = int.Parse((await command.ExecuteScalarAsync()).ToString());
                invoiceNumber = invoiceNumber + 1;
                command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO `invoice_number` (
		                            `IdReceipt`,
                                    `number`,
                                    `year`)
                                    VALUES (
			                            $selectedIdReceipt,
                                        $invoiceNumber,
                                        $selectedYear
                                    ) ";
                command.Parameters.Add(new SqliteParameter("$selectedIdReceipt", invoiceData.IdReceipt));
                command.Parameters.Add(new SqliteParameter("$invoiceNumber", invoiceNumber));
                command.Parameters.Add(new SqliteParameter("$selectedYear", invoiceData.Year));
                await command.ExecuteNonQueryAsync();

                command = connection.CreateCommand();
                command.CommandText = @"UPDATE `payment_receipt`
			                        SET `InvoiceNumber` = $invoiceNumber,
				                        `PaymentDate` = CURRENT_TIMESTAMP,
                                        `CustomerName` = $customerName,
                                        `CustomerFiscalCode` = $customerFiscalCode,
                                        `CustomerAddress` = $customerAddress,
                                        `CostAmount` = $costAmount,
                                        `IssuedBy` = $issuedBy,
                                        `PaymentType` = $paymentType,
                                        `Description` = $paymentDescription
			                        WHERE
				                        `Id` = $selectedIdReceipt ";

                command.Parameters.Add(new SqliteParameter("$invoiceNumber", $"{invoiceData.Year}_" + invoiceNumber.ToString().PadLeft(4, '0')));
                command.Parameters.Add(new SqliteParameter("$selectedIdReceipt", invoiceData.IdReceipt));
                command.Parameters.Add(new SqliteParameter("$customerName", invoiceData.CustomerName));
                command.Parameters.Add(new SqliteParameter("$customerFiscalCode", invoiceData.CustomerFiscalCode));
                command.Parameters.Add(new SqliteParameter("$customerAddress", invoiceData.CustomerAddress));
                command.Parameters.Add(new SqliteParameter("$costAmount", receipt.PaymentReceiptDetails.Sum(x => x.CostAmount)));
                command.Parameters.Add(new SqliteParameter("$issuedBy", new Guid(_claimPrincipal.Value.Identity.Name)));
                command.Parameters.Add(new SqliteParameter("$paymentType", invoiceData.PaymentType));
                command.Parameters.Add(new SqliteParameter("$paymentDescription", invoiceData.Description));

                await command.ExecuteNonQueryAsync();
                await trx.CommitAsync();
            } 
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                throw ex;
            }
        }

        private async Task InvokeInvoiceSP(InvoiceRequestData invoiceData, PaymentReceipt receipt)
        {
            await _dbCtx.Database.OpenConnectionAsync();
            var connection = _dbCtx.Database.GetDbConnection();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Save_Invoice";
            command.Parameters.Add(new MySqlParameter("selectedIdReceipt", invoiceData.IdReceipt));
            command.Parameters.Add(new MySqlParameter("selectedYear", invoiceData.Year));
            command.Parameters.Add(new MySqlParameter("customerName", invoiceData.CustomerName));
            command.Parameters.Add(new MySqlParameter("customerFiscalCode", invoiceData.CustomerFiscalCode));
            command.Parameters.Add(new MySqlParameter("customerAddress", invoiceData.CustomerAddress));
            command.Parameters.Add(new MySqlParameter("costAmount", receipt.PaymentReceiptDetails.Sum(x => x.CostAmount)));
            command.Parameters.Add(new MySqlParameter("issuedBy", new Guid(_claimPrincipal.Value.Identity.Name)));
            command.Parameters.Add(new MySqlParameter("paymentType", invoiceData.PaymentType));
            command.Parameters.Add(new MySqlParameter("paymentDescription", invoiceData.Description));
            var res = (int?)command.ExecuteScalar();
            await _dbCtx.Database.CloseConnectionAsync();
            if (res.HasValue)
            {
                var desc = "No error description avaialle";
                if (res.Value == 1062)
                {
                    desc = "Duplicate invoice number!!";
                }
                throw new Exception($"Unable to create ad store invoice number. ERR-NO={res} {desc}");
            }
        }
    }
}
