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
    public class Product : BaseEntity
    {
        /* 
         CREATE TABLE `product` (
           `Id` char(36) NOT NULL,
           `Name` varchar(256) NOT NULL,
           `Description` longtext,
           `XCreateDate` datetime DEFAULT NULL,
           `XUpdateDate` datetime DEFAULT NULL,
           `XDeleteDate` datetime DEFAULT NULL,
           `XLastEditUser` varchar(45) DEFAULT NULL,
           `XCreationUser` varchar(45) DEFAULT NULL,
           PRIMARY KEY (`Id`),
           UNIQUE KEY `Id_UNIQUE` (`Id`)
         ) ENGINE=InnoDB DEFAULT CHARSET=utf8;

         */

        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public Guid? IdTeacher { get; set; }
        public virtual User Teacher {get; set;}
        public virtual ICollection<ProductInstance> ProductInstances { get; set; }
        public virtual ICollection<ProductSession> ProductSessions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Product()
        {
            ProductInstances = new List<ProductInstance>();
            ProductSessions = new List<ProductSession>();
        }
    }
}
