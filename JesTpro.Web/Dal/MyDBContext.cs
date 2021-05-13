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
using Microsoft.EntityFrameworkCore;
using jt.jestpro.dal.Entities;
using jt.jestpro.dal.Mappers;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace jt.jestpro.dal
{
    public class MyDBContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MyDBContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Image> Images { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInstance> ProductInstances { get; set; }
        public DbSet<ProductSession> ProductSessions { get; set; }
        public DbSet<ProductSessionAttendance> ProductSessionAttendances { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<CustomerProductInstance> CustomerProductInstances { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<PaymentReceiptDetail> PaymentReceiptDetails { get; set; }
        public DbSet<Translate> Translates { get; set; }
        public DbSet<SqlMigration> Migrations { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<MassiveRequest> MassiveRequests { get; set; }

        public string Token { get; set; }

        public static DateTime ForceUTCKind(DateTime dt)
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ImageMap());
            builder.ApplyConfiguration(new AuditMap());
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new RoleMap());
            builder.ApplyConfiguration(new ProductMap());
            builder.ApplyConfiguration(new ProductSessionMap());
            builder.ApplyConfiguration(new ProductSessionAttendanceMap());
            builder.ApplyConfiguration(new ProductInstanceMap());
            builder.ApplyConfiguration(new CustomerMap());
            builder.ApplyConfiguration(new CustomerTypeMap());
            builder.ApplyConfiguration(new CustomerProductInstanceMap());
            builder.ApplyConfiguration(new PaymentReceiptMap());
            builder.ApplyConfiguration(new CreditNoteMap());
            builder.ApplyConfiguration(new PaymentReceiptDetailMap());
            builder.ApplyConfiguration(new TranslateMap());
            builder.ApplyConfiguration(new SqlMigrationMap());
            builder.ApplyConfiguration(new SettingMap());
            builder.ApplyConfiguration(new ReportMap());
            builder.ApplyConfiguration(new AttachmentMap());
            builder.ApplyConfiguration(new MassiveRequestMap());

            builder.Entity<Image>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<Product>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<ProductInstance>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<ProductSession>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<ProductSessionAttendance>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<Customer>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<CustomerType>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<CustomerProductInstance>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<PaymentReceipt>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<CreditNote>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<PaymentReceiptDetail>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<Report>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<Attachment>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
            builder.Entity<MassiveRequest>(entity => entity.HasQueryFilter(x => !x.XDeleteDate.HasValue));
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var auditEntries = OnBeforeSaving();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override int SaveChanges()
        {
            return SaveChanges(true);
            //var auditEntries = OnBeforeSaving();
            //var result = base.SaveChanges();
            //OnAfterSaveChanges(auditEntries);
            //return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditEntries = OnBeforeSaving();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in entries)
            {
                //AUDIT
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                if (entry.Entity is ITrackable trackable)
                {
                    var now = DateTime.UtcNow;
                    var user = GetCurrentUser();
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.XUpdateDate = now;
                            trackable.XLastEditUser = user;
                            break;

                        case EntityState.Added:
                            trackable.XUpdateDate = now;
                            trackable.XLastEditUser = user;
                            trackable.XCreateDate = now;
                            trackable.XCreationUser = user;
                            break;
                    }
                }

                var auditEntry = new AuditEntry(entry);
                auditEntry.User = GetCurrentUser();
                auditEntry.TableName = entry.Metadata.GetTableName();
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private string GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var authenticatedUserName = httpContext.User.Identity.Name;
                if (authenticatedUserName != null)
                {
                    return authenticatedUserName;
                }
                else
                {
                    var authenticatedUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);
                    if (authenticatedUserId != null)
                    {
                        return authenticatedUserId.Value;
                    }
                    else
                    {
                        return "Miss";
                    }
                }
                
                // TODO use name to set the shadow property value like in the following post: https://www.meziantou.net/2017/07/03/entity-framework-core-generate-tracking-columns
            }
            else
            {
                return "Unknown";
            }
            //return "Admin"; // TODO implement your own logic

            // If you are using ASP.NET Core, you should look at this answer on StackOverflow
            // https://stackoverflow.com/a/48554738/2996339
        }

        
        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                Audits.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }
    }

    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string User { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.User = User;
            audit.TableName = TableName;
            audit.DateTime = DateTime.UtcNow;
            audit.KeyValues = JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            return audit;
        }
    }
}
