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
    public class ProductInstanceDto : ProductInstanceEditDto
    {

        public ProductDto Product { get; set; }
        public List<CustomerProductInstanceDto> CustomerProductInstances { get; set; }

        public ProductInstanceDto()
        {
            //Product = new ProductDto();
            CustomerProductInstances = new List<CustomerProductInstanceDto>();
        }
    }

    public class ProductInstanceFilterDto : ProductInstanceEditDto
    {
    }
    public class ProductInstanceEditDto : BaseDto
    {
        public Guid IdProduct { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }
        public int Weeks { get; set; }
        public int Months { get; set; }
        public int Years { get; set; }
        public decimal Price { get; set; }
    }
}
