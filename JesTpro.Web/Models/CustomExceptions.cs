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
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message)
                : base(message)
        {
        }

        public UnauthorizedException(string message, Exception inner)
                : base(message, inner)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
                : base(message)
        {
        }

        public NotFoundException(string message, Exception inner)
                : base(message, inner)
        {
        }
    }

    public class ErrorModel
    {
        public Dictionary<string, string> errors { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string traceId { get; set; }

        public ErrorModel()
        {
            errors = new Dictionary<string, string>();
        }

    }
}
