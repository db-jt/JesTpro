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
    public static class EmMassiveRequest
    {
        public static MassiveRequestDto ToDto(this MassiveRequest e)
        {
            if (e == null)
                return null;

            var res = new MassiveRequestDto();
            res.Id = e.Id;
            res.Description = e.Description;
            res.Error = e.Error;
            res.FileToImport = e.FileToImport;
            res.ImportStatus = (Models.ImportStatus)e.ImportStatus;
            res.ImportType = (Models.ImportType)e.ImportType;
            res.LastExecution = e.LastExecution;
            return res;
        }

        public static MassiveRequest ToEntity(this MassiveRequestEditDto e)
        {
            if (e == null)
                return null;

            var res = new MassiveRequest();
            res.Id = e.Id;
            res.Description = e.Description;
            res.ImportStatus = dal.Entities.ImportStatus.New;
            return res;
        }
    }
}
