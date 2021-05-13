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

using ClosedXML.Excel;
using jt.jestpro.dal;
using jt.jestpro.dal.Entities;
using jt.jestpro.Helpers;
using jt.jestpro.Helpers.ExtensionMethods;
using jt.jestpro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IReportService : ICRUDService<ReportDto, ReportFilterDto, ReportEditDto>
    {
        Task<List<string>> GetLastLogs();
        Task<PaymentReceiptDto[]> GetReportCashDesk(ReportCashDeskFilter filter);
        Task<byte[]> ExportInExcel(ReportExcelDto parameters);

    }
    public class ReportService : IReportService
    {
        MyDBContext _dbCtx;
        ILogger<ReportService> _logger;
        AppSettings _appSettings;

        public ReportService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<ReportService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete Report for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Reports.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ReportDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get Report for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<Report> GetInner(Guid id)
        {
            return await _dbCtx.Reports.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ReportDto[]> GetList(ReportFilterDto filter)
        {
            _logger.LogDebug($"Calling getList Report");

            IQueryable<Report> query = _dbCtx.Reports;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }
            
            if (filter.Enabled.HasValue)
            {
                query = query.Where(x => x.Enabled == filter.Enabled.Value);
            }

            if (filter.Family.HasValue)
            {
                query = query.Where(x => x.Family == (dal.Entities.ReportFamily)filter.Family.Value);
            }

            var result = await query.ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ReportDto> Save(ReportEditDto itemToEdit)
        {
            Report res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update Report for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"Report with id={itemToEdit.Id} not exists!");
                }
                res.Name = itemToEdit.Name;
                res.Description = itemToEdit.Description;
                res.Value = itemToEdit.Value;
                res.ColumnMap = itemToEdit.ColumnMap;
                res.ParameterMap = itemToEdit.ParameterMap;
                res.Enabled = itemToEdit.Enabled;
                res.Family = (dal.Entities.ReportFamily)itemToEdit.Family;
                _dbCtx.Reports.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert Report for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.Reports.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

        public async Task<List<string>> GetLastLogs()
        {
            var res = new List<string>();
            var logPath = _appSettings.LogPath;
            var sortedFiles = Directory.GetFiles(logPath).OrderByDescending(f => f);

            res.AddRange(sortedFiles.Take(15).Select(x => new FileInfo(x).Name));
            var mainLog = sortedFiles.Where(x => x.ToLower().EndsWith(".log")).FirstOrDefault();
            if (mainLog != null)
            {
                if (res.Count(x => x.ToLower().EndsWith(".log")) == 0)
                {
                    res.Insert(0, new FileInfo(mainLog).Name);
                }

                var logContent = System.IO.File.ReadAllText(mainLog);
                res.Insert(0, logContent);
            }
            else
            {
                res.Insert(0, "No active log found!");
            }
            return res;
        }

        public async Task<PaymentReceiptDto[]> GetReportCashDesk(ReportCashDeskFilter filter)
        {
            IQueryable<PaymentReceipt> query = _dbCtx.PaymentReceipts;
            query = query.Where(x => !x.XDeleteDate.HasValue);
            var noFilter = string.IsNullOrWhiteSpace(filter.CategoryName)
                && !filter.IdTeacher.HasValue
                && !filter.StartDate.HasValue
                && !filter.EndDate.HasValue
                && !filter.IdIssuer.HasValue
                && !filter.IdProductDetails.HasValue
                && !filter.IdProduct.HasValue;

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

            query = query.Where(x => x.PaymentDate.HasValue && (x.PaymentDate.Value >= filter.StartDate.Value && x.PaymentDate.Value <= filter.EndDate.Value));

            if (filter.IdIssuer.HasValue)
            {
                query = query.Where(x => x.IssuedBy.HasValue && x.IssuedBy == filter.IdIssuer);
            }

            if (filter.IdProductDetails.HasValue)
            {
                query = query.Where(x => x.PaymentReceiptDetails.Select(xx => xx.IdResource).Contains(filter.IdProductDetails.Value));
            }


            if (filter.IdProduct.HasValue)
            {
                var compliantDetails = await _dbCtx.ProductInstances.Where(x => !x.XDeleteDate.HasValue && x.IdProduct == filter.IdProduct)
                    .Select(xx => xx.Id).ToArrayAsync();
                var compliantRecipts = await _dbCtx.PaymentReceiptDetails.Where(x => compliantDetails.Contains(x.IdResource)).Select(x => x.IdReceipt).ToArrayAsync();
                query = query.Where(x => compliantRecipts.Contains(x.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter.CategoryName))
            {
                var compliantDetails = await _dbCtx.ProductInstances.Where(x => !x.XDeleteDate.HasValue && x.Product.CategoryName == filter.CategoryName)
                    .Select(xx => xx.Id).ToArrayAsync();
                var compliantRecipts = await _dbCtx.PaymentReceiptDetails.Where(x => compliantDetails.Contains(x.IdResource)).Select(x => x.IdReceipt).ToArrayAsync();
                query = query.Where(x => compliantRecipts.Contains(x.Id));
            }

            if (filter.IdTeacher.HasValue)
            {
                var compliantDetails = await _dbCtx.ProductInstances.Where(x => !x.XDeleteDate.HasValue && x.Product.IdTeacher == filter.IdTeacher)
                    .Select(xx => xx.Id).ToArrayAsync();
                var compliantRecipts = await _dbCtx.PaymentReceiptDetails.Where(x => compliantDetails.Contains(x.IdResource)).Select(x => x.IdReceipt).ToArrayAsync();
                query = query.Where(x => compliantRecipts.Contains(x.Id));
            }

            if (noFilter)
            {
                query = query.OrderByDescending(x => x.InvoiceNumber);
            }
            else
            {
                query = query.OrderByDescending(x => x.PaymentDate);
            }

            var res = await query.ToArrayAsync();
            return res.Select(x => x.ToDto()).ToArray();
        }

        public async Task<byte[]> ExportInExcel(ReportExcelDto parameters)
        {
            var timeZoneSett = await _dbCtx.Settings.FirstOrDefaultAsync(x => x.Key == "company.time-zone");
            var timeZone = "";
            if (timeZone == null)
            {
                _logger.LogWarning($"No timezone specified in config, assuming localTimeZone {TimeZoneInfo.Local}");
            }
            else
            {
                timeZone = timeZoneSett.Value;
            }
            var timeZoneOffSet = Utils.GetTimeOffset(timeZone);
            var dateTimePattern = await _dbCtx.Settings.FirstOrDefaultAsync(x => x.Key == "company.report-excel-date-pattern");
            if (dateTimePattern == null)
            {
                throw new Exception("Missing date pattern for excel report. Please check your company configuration");
            }
            var idReport = parameters.Id;
            var report = await _dbCtx.Reports.FindAsync(idReport);
            if (report == null)
            {
                throw new Exception($"No report found with id='{idReport}'");
            }

            var defParameters = JsonConvert.DeserializeObject<ReportExcelParameterValue[]>(report.ParameterMap);
            var defColumnAlias = JsonConvert.DeserializeObject<ReportExcelColumnMap[]>(report.ColumnMap);
            var reqPars = defParameters.Where(x => x.Required).ToArray();

            foreach (var par in reqPars)
            {
                var check = parameters.Values.Where(x => x.Name.Equals(par.Name, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(x.Value)).Count();
                if (check == 0)
                {
                    throw new Exception($"Missing required param='{par.Name}'");
                }
            }

            var query = report.Value;
            var genericParams = parameters.Values.Where(x => x.Type == "generic").ToArray();
            foreach (var genPar in genericParams)
            {
                query = query.Replace(genPar.Name, string.IsNullOrEmpty(genPar.Value)?"": genPar.Value);
            }

            var typedParams = parameters.Values.Where(x => x.Type != "generic").ToArray();
            
            using (var cmd = _dbCtx.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = query;
                foreach (var typedPar in typedParams)
                {
                    if (typedPar.Type.Equals("date",StringComparison.InvariantCultureIgnoreCase))
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = typedPar.Name;
                        if (DateTime.TryParse(typedPar.Value, out DateTime dPar))
                        {
                            param.Value = dPar;
                        } 
                        else
                        {
                            if (typedPar.Required)
                            {
                                throw new Exception($"Unable to convert'{typedPar.Name}' to DateTime");
                            }
                            param.Value = null;
                        }
                        cmd.Parameters.Add(param);
                    }
                    else if (typedPar.Type.Equals("number", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = typedPar.Name;
                        if (decimal.TryParse(typedPar.Value, out decimal nPar))
                        {
                            param.Value = nPar;
                        }
                        else
                        {
                            if (typedPar.Required)
                            {
                                throw new Exception($"Unable to convert'{typedPar.Name}' to number");
                            }
                        }
                        
                        cmd.Parameters.Add(param);
                    }
                    else if (typedPar.Type.Equals("boolean", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = typedPar.Name;
                        if (bool.TryParse(typedPar.Value, out bool bPar))
                        {
                            param.Value = bPar;
                        }
                        else
                        {
                            if (typedPar.Required)
                            {
                                throw new Exception($"Unable to convert'{typedPar.Name}' to boolean");
                            }
                        }
                        
                        cmd.Parameters.Add(param);
                    }
                    else // text value
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = typedPar.Name;
                        param.Value = typedPar.Value;
                        cmd.Parameters.Add(param);
                    }
                }
                await _dbCtx.Database.OpenConnectionAsync();
                var objReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                using (var workbook = new XLWorkbook())
                {

                    var worksheet = workbook.Worksheets.Add(report.Name);

                    //Add Header
                    var colIdx = 1;
                    for (int count = 0; count < objReader.FieldCount; count++)
                    {
                        var fieldName = objReader.GetName(count);
                        if (parameters.SelectedFields == null || parameters.SelectedFields.Count == 0 || parameters.SelectedFields.Contains(fieldName))
                        {
                            var alias = defColumnAlias.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
                            if (alias != null)
                            {
                                fieldName = alias.Alias;
                            }

                            worksheet.Cell(1, colIdx).Value = fieldName;
                            colIdx++;
                        }
                    }
                    int rowIndex = 2;
                   
                    while (objReader.Read())
                    {
                        colIdx = 1;
                        //Add Body
                        for (int col = 0; col < objReader.FieldCount; col++)
                        {
                            var fieldName = objReader.GetName(col);
                            if (parameters.SelectedFields == null || parameters.SelectedFields.Count == 0 || parameters.SelectedFields.Contains(fieldName))
                            {
                                var fieldValue = objReader.GetValue(col);
                                if (objReader.GetFieldType(col) == typeof(DateTime) && fieldValue != null && fieldValue != System.DBNull.Value)
                                {
                                    worksheet.Cell(rowIndex, colIdx).SetDataType(XLDataType.DateTime);
                                    worksheet.Cell(rowIndex, colIdx).Style.NumberFormat.Format = dateTimePattern.Value;
                                    var dateValue = (DateTime)fieldValue;
                                    dateValue.Add(timeZoneOffSet * (-1));
                                    worksheet.Cell(rowIndex, colIdx).Value = dateValue;
                                }
                                else
                                {
                                    worksheet.Cell(rowIndex, colIdx).Value = fieldValue;
                                }
                                colIdx++;
                            }
                        }
                        rowIndex++;
                    }

                    // return excel stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            
        }
    
    }
}
