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

using jt.jestpro.dal.Entities;
using jt.jestpro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Helpers.ExtensionMethods
{
    public static class EmReport
    {
        public static ReportDto ToDto(this Report e)
        {
            if (e == null)
                return null;

            var res = new ReportDto();
            res.Id = e.Id;
            res.Name = e.Name;
            res.Description = e.Description;
            res.ColumnMap = e.ColumnMap;
            res.Value = e.Value;
            res.ParameterMap = e.ParameterMap;
            res.Enabled = e.Enabled;
            res.Family = (Models.ReportFamily)e.Family;
            return res;
        }

        public static Report ToEntity(this ReportEditDto e)
        {
            if (e == null)
                return null;

            var res = new Report();
            res.Id = e.Id;
            res.Name = e.Name;
            res.Description = e.Description;
            res.ColumnMap = e.ColumnMap;
            res.Value = e.Value;
            res.ParameterMap = e.ParameterMap;
            res.Family = (dal.Entities.ReportFamily)e.Family;
            res.Enabled = e.Enabled;
            return res;
        }
    }
}
