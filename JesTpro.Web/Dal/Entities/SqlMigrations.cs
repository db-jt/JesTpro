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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.dal.Entities
{
    public class SqlMigration 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime XCreateDate { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }

        public SqlMigration()
        {
          
        }
    }
}
