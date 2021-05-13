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
    public class ProductInstance : BaseEntity
    {
        /* 
        CREATE TABLE `product_instance` (
          `Id` char(36) NOT NULL,
          `IdProduct` char(36) NOT NULL,
          `Name` varchar(256) NOT NULL,
          `Description` longtext,
          `Days` int(11) NOT NULL DEFAULT '0',
          `Weeks` int(11) NOT NULL DEFAULT '0',
          `Months` int(11) NOT NULL DEFAULT '0',
          `Years` int(11) NOT NULL DEFAULT '0',
          `Price` decimal(15,2) NOT NULL DEFAULT '0.00',
          `XCreateDate` datetime DEFAULT NULL,
          `XUpdateDate` datetime DEFAULT NULL,
          `XDeleteDate` datetime DEFAULT NULL,
          `XLastEditUser` varchar(45) DEFAULT NULL,
          `XCreationUser` varchar(45) DEFAULT NULL,
          PRIMARY KEY (`Id`),
          UNIQUE KEY `Id_UNIQUE` (`Id`)
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8;

         */
        public Guid IdProduct { get; set; }
        public virtual Product Product { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }
        public int Weeks { get; set; }
        public int Months { get; set; }
        public int Years { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<CustomerProductInstance> CustomerProductInstances { get; set; }

        public ProductInstance()
        {
            CustomerProductInstances = new List<CustomerProductInstance>();
        }
    }
}
