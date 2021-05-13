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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Models
{
    public enum ReportFamily
    {
        Excel,
        Graph
    }
    public class ReportDto : ReportEditDto
    {
        public ReportDto()
        {
        }
    }

    public class ReportFilterDto : BaseDto { 
        public string[] ColumnDisabled { get; set; }
        public bool? Enabled { get; set; }
        public ReportFamily? Family { get; set; }

    }

    public class ReportEditDto : BaseDto {
        public string Name { get; set; }
        public ReportFamily Family { get; set; }
        public string Description { get; set; }
        public string ColumnMap { get; set; }
        public string ParameterMap { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; }
    }

    public class ReportExcelDto
    {
        public Guid Id { get; set; }
        public ReportExcelParameterValue[] Values { get; set; }
        public List<string> SelectedFields { get; set; }

        public ReportExcelDto()
        {
            Values = new ReportExcelParameterValue[0];
        }

    }

    public class ReportExcelParameterValue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Value { get; set; }
    }

    public class ReportExcelColumnMap
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class SingleChartDataDto
    {
        public string Name { get; set; }
        public object Value { get; set; }

        // il min/max danno problemi in fase di render dei grafici, se occorrono vanno rivisti
        //public object Min { get; set; }
        //public object Max { get; set; }
    }

    public class MultiChartDataDto
    {
        public string Name { get; set; }
        public List<SingleChartDataDto> Series { get; set; }
        public MultiChartDataDto()
        {
            Series = new List<SingleChartDataDto>();
        }
    }


    public class ReportCashDeskFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? IdIssuer { get; set; }
        public Guid? IdProduct { get; set; }
        public Guid? IdProductDetails { get; set; }
        public string CategoryName { get; set; }
        public Guid? IdTeacher { get; set; }
    }

}
