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
    public static class EmAttachment
    {
        public static AttachmentDto ToDto(this Attachment e)
        {
            if (e == null)
                return null;

            var res = new AttachmentDto();
            res.Id = e.Id;
            res.Name = e.Name;
            res.Size = e.Size;
            res.IdResource = e.IdResource;
            return res;
        }

        public static Attachment ToEntity(this AttachmentEditDto e)
        {
            if (e == null)
                return null;

            var res = new Attachment();
            res.Id = e.Id;
            res.Name = e.Name;
            res.IdResource = e.IdResource;
            res.FullPath = e.FullPath;
            res.Size = e.Size;
            return res;
        }
    }
}
