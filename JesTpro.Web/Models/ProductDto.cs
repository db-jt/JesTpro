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
    public class ProductDto : ProductEditDto
    {
        public List<ProductInstanceDto> ProductInstances { get; set; }
        public UserDto Teacher { get; set; }

        public ProductDto()
        {
            ProductInstances = new List<ProductInstanceDto>();
        }
    }

    public class ProductFilterDto : ProductEditDto
    {
    }
    public class ProductEditDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }

        public Guid? IdTeacher { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
