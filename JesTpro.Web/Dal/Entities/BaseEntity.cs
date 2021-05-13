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

namespace jt.jestpro.dal.Entities
{
    public interface ITrackable
    {
        DateTime? XCreateDate { get; set; }
        DateTime? XUpdateDate { get; set; }
        DateTime? XDeleteDate { get; set; }
        string XLastEditUser { get; set; }
        string XCreationUser { get; set; }
    }
    public class BaseEntity : ITrackable
    {
        public Guid Id { get; set; }
        public DateTime? XCreateDate { get; set; }
        public DateTime? XUpdateDate { get; set; }
        public DateTime? XDeleteDate { get; set; }
        public string XLastEditUser { get; set; }
        public string XCreationUser { get; set; }

        public BaseEntity()
        {
           
        }
    }
}
