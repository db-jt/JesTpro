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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jt.jestpro.dal.Mappers
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.HasIndex(e => e.Id)
                        .HasName("Id_UNIQUE")
                        .IsUnique();

            builder.HasOne(e => e.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.IdRole);

        }
    }
}
