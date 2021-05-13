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
    public interface ICreditNoteService : ICRUDService<CreditNoteDto, CreditNoteFilterDto, CreditNoteEditDto>
    {
        Task<string> GetPdfCreditNotePath(Guid creditNoteId);

    }
    public class CreditNoteService : ICreditNoteService
    {
        MyDBContext _dbCtx;
        ILogger<CreditNoteService> _logger;
        IOptions<AppSettings> _appSettings;
        private readonly Lazy<ITemplateHelperService> _templateService;
        private readonly Lazy<IHtmlToPDF> _pdfService;
        private readonly Lazy<ClaimsPrincipal> _claimPrincipal;


        public CreditNoteService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, Lazy<ITemplateHelperService> templateService, Lazy<IHtmlToPDF> pdfService, Lazy<ClaimsPrincipal> claimPrincipal, ILogger<CreditNoteService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
            _templateService = templateService;
            _pdfService = pdfService;
            _claimPrincipal = claimPrincipal;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete CreditNote for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.CreditNotes.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<CreditNoteDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get CreditNote for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<CreditNote> GetInner(Guid id)
        {
            return await _dbCtx.CreditNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CreditNoteDto[]> GetList(CreditNoteFilterDto filter)
        {
            _logger.LogDebug($"Calling getList CreditNote");

            IQueryable<CreditNote> query = _dbCtx.CreditNotes;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }
            if (filter.IdReceipt != Guid.Empty)
            {
                query = query.Where(x => x.IdReceipt == filter.IdReceipt);
            }
           
            var result = await query.OrderByDescending(x => x.IssueDate).ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<CreditNoteDto> Save(CreditNoteEditDto itemToEdit)
        {
            CreditNote res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update CreditNote for id=[{itemToEdit.Id}]");
                throw new Exception("Credit notes are not editable!!!");
            }
            else
            {
                var receiptData = await _dbCtx.PaymentReceipts.FindAsync(itemToEdit.IdReceipt);
                if (receiptData == null)
                {
                    throw new Exception($"Unable to find receipt with id {itemToEdit.IdReceipt}");
                } 
                else if (string.IsNullOrWhiteSpace(receiptData.InvoiceNumber))
                {
                    throw new Exception($"The selected receipt has no invoce number for credit note generation ({itemToEdit.IdReceipt})");
                }

                var checkCreditNote = _dbCtx.CreditNotes.Where(x => x.IdReceipt == itemToEdit.IdReceipt).Count();
                if (checkCreditNote > 0)
                {
                    throw new Exception($"A credit note alredy exist for this receipt!");
                }

                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                res.IssuedBy = new Guid(_claimPrincipal.Value.Identity.Name);
                res.InvoiceNumber = receiptData.InvoiceNumber;
                res.InvoiceDate = receiptData.PaymentDate.Value;

                _logger.LogDebug($"Calling Insert CreditNote for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.CreditNotes.AddAsync(res);
                _dbCtx.SaveChanges();

                var creditNoteData = await Get(res.Id);
                if (!creditNoteData.Year.HasValue)
                {
                    creditNoteData.Year = DateTime.UtcNow.Year;
                }

                if (_appSettings.Value.UseSqLite)
                {
                    await CreateCreditNote(creditNoteData);
                }
                else
                {
                    await InvokeCreditNoteSP(creditNoteData, res);
                }

                //remove paid products
                var productInstances = await _dbCtx.CustomerProductInstances.Where(x => x.IdReceipt == receiptData.Id).ToArrayAsync();
                foreach (var pInstance in productInstances)
                {
                    pInstance.IdReceipt = null;
                    pInstance.PaymentStatus = dal.Entities.PaymentStatus.Aborted;
                    _dbCtx.CustomerProductInstances.Update(pInstance);
                }

                //remove paid fees
                var customer = _dbCtx.Customers.Find(receiptData.IdCustomer);
                if (customer != null)
                {
                    var fees = receiptData.PaymentReceiptDetails.Where(x => x.ReceiptDetailType == dal.Entities.ReceiptDetailType.Fee);
                    foreach (var fee in fees)
                    {
                    
                        var now = DateTime.UtcNow;
                        customer.MembershipLastPayDate = now;
                        if (customer.MembershipFeeExpiryDate.HasValue)
                        {
                            customer.MembershipFeeExpiryDate = customer.MembershipFeeExpiryDate.Value.AddYears(-1);
                            _dbCtx.Customers.Update(customer);
                        }
                    }
                }

                await _dbCtx.SaveChangesAsync();
            }
            return res.ToDto();
        }

        private async Task<bool> GeneratePdfCreditNote(Guid creditNoteId)
        {
            var viewModel = (await this.GetInner(creditNoteId)).ToDto(true);
            viewModel.Settings = await _dbCtx.Settings.Where(x => x.Key.ToLower().StartsWith("company")).Select(x => x.ToDto()).ToArrayAsync(); ;
            var user = _dbCtx.Users.Find(new Guid(_claimPrincipal.Value.Identity.Name));
            var lang = string.IsNullOrWhiteSpace(user.Lang) ? _appSettings.Value.DefaultLocale : user.Lang.ToUpper();
            var html = await _templateService.Value.GetTemplateHtmlAsStringAsync($"CreditNote/CreditNote_{lang}.cshtml", viewModel);
            //var html = await _razorViewToStringRenderer.Value.RenderViewToStringAsync($"/Views/Templates/Invoice/Invoice_{_appSettings.Value.DefaultLocale}.cshtml", model.ToDto());
            var receiptPath = await _pdfService.Value.CreateReport(html, viewModel.CreditNoteNumber, _appSettings.Value.PdfSettings);
            var entity = await this.GetInner(creditNoteId);
            entity.CreditNotePath = receiptPath.Replace("\\", "/");
            _dbCtx.CreditNotes.Update(entity);
            await _dbCtx.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetPdfCreditNotePath(Guid creditNoteId)
        {
            var creditNote = await this.GetInner(creditNoteId);
            if (string.IsNullOrWhiteSpace(creditNote.CreditNotePath) || !File.Exists(creditNote.CreditNotePath))
            {
                await GeneratePdfCreditNote(creditNoteId);
                creditNote = await this.GetInner(creditNoteId);
            }

            if (string.IsNullOrWhiteSpace(creditNote.CreditNotePath))
            {
                _logger.LogError($"No pdf found for creditNoteId=[{creditNoteId}]");
                throw new Exception("No pdf found for this creditNote");
            }
            else
            {
                return creditNote.CreditNotePath;
            }
        }

        private async Task CreateCreditNote(CreditNoteDto creditNoteData)
        {
            // SPECIFIC FOR SQLite DB
            var trx = await _dbCtx.Database.BeginTransactionAsync();
            await _dbCtx.Database.OpenConnectionAsync();
            var connection = _dbCtx.Database.GetDbConnection();
            try { 
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT COALESCE(MAX(number),0)
                                        FROM `creditnote_number` 
                                        WHERE `year` = $selectedYear ";

                command.Parameters.Add(new SqliteParameter("$selectedYear", creditNoteData.Year));

                var creditNoteNumber = int.Parse((await command.ExecuteScalarAsync()).ToString());
                creditNoteNumber = creditNoteNumber + 1;
                command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO `creditnote_number` (
		                            `IdCreditNote`,
                                    `number`,
                                    `year`)
                                    VALUES (
			                            $selectedIdCreditNote,
                                        $cnNumber,
                                        $selectedYear
                                    );";
                command.Parameters.Add(new SqliteParameter("$selectedIdCreditNote", creditNoteData.Id));
                command.Parameters.Add(new SqliteParameter("$cnNumber", creditNoteNumber));
                command.Parameters.Add(new SqliteParameter("$selectedYear", creditNoteData.Year));
                await command.ExecuteNonQueryAsync();

                command = connection.CreateCommand();
                command.CommandText = @" UPDATE `credit_note`
			                            SET `CreditNoteNumber` = $cnNumber,
				                            `IssueDate` = CURRENT_TIMESTAMP,
                                            `IssuedBy` = $issuedBy
			                            WHERE
				                            `Id` = $selectedIdCreditNote";

                command.Parameters.Add(new SqliteParameter("$cnNumber", $"NC_{creditNoteData.Year}_" + creditNoteNumber.ToString().PadLeft(4,'0')));
                command.Parameters.Add(new SqliteParameter("$issuedBy", new Guid(_claimPrincipal.Value.Identity.Name)));
                command.Parameters.Add(new SqliteParameter("$selectedIdCreditNote", creditNoteData.Id));

                await command.ExecuteNonQueryAsync();
                await trx.CommitAsync();
            } 
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                throw ex;
            }
}

        private async Task InvokeCreditNoteSP(CreditNoteDto creditNoteData, CreditNote res)
        {
            //Create the credit note number
            await _dbCtx.Database.OpenConnectionAsync();
            var connection = _dbCtx.Database.GetDbConnection();

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Save_CreditNote";
            command.Parameters.Add(new MySqlParameter("selectedIdCreditNote", creditNoteData.Id));
            command.Parameters.Add(new MySqlParameter("selectedYear", creditNoteData.Year));
            command.Parameters.Add(new MySqlParameter("issuedBy", new Guid(_claimPrincipal.Value.Identity.Name)));
            var cnNum = (int?)command.ExecuteScalar();
            await _dbCtx.Database.CloseConnectionAsync();
            if (cnNum.HasValue)
            {
                var desc = "No error description avaialle";
                if (cnNum.Value == 1062)
                {
                    desc = "Duplicate credit note number!!";
                }
                _dbCtx.CreditNotes.Remove(res);
                throw new Exception($"Unable to create ad store credit note number. ERR-NO={cnNum} {desc}");
            }
        }
    }
}
