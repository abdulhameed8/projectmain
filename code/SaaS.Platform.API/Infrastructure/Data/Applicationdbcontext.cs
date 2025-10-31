using Microsoft.EntityFrameworkCore;
using SaaS.Platform.API.Domain.Entities;

namespace SaaS.Platform.API.Infrastructure.Data
{
    /// <summary>
    /// Main database context for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for entities
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.CustomerId)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.CustomerCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Mobile)
                    .HasMaxLength(20);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.City)
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasMaxLength(100);

                entity.Property(e => e.Country)
                    .HasMaxLength(100);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20);

                entity.Property(e => e.TaxId)
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerSegment)
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");

                entity.Property(e => e.CustomerSource)
                    .HasMaxLength(50);

                entity.Property(e => e.Tags)
                    .HasMaxLength(500);

                entity.Property(e => e.PreferredLanguage)
                    .HasMaxLength(10)
                    .HasDefaultValue("en");

                entity.Property(e => e.PreferredContactMethod)
                    .HasMaxLength(20);

                entity.Property(e => e.CreditLimit)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Indexes
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("IX_Customers_TenantId");

                entity.HasIndex(e => e.CustomerCode)
                    .HasDatabaseName("IX_Customers_CustomerCode");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Customers_Email");

                entity.HasIndex(e => e.CustomerStatus)
                    .HasDatabaseName("IX_Customers_CustomerStatus");

                entity.HasIndex(e => new { e.TenantId, e.CustomerCode })
                    .IsUnique()
                    .HasDatabaseName("UQ_Customers_TenantCode");

                // Ignore computed properties
                entity.Ignore(e => e.FullName);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set audit fields
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is Domain.Common.BaseEntity baseEntity)
                    {
                        baseEntity.CreatedDate = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Domain.Common.BaseEntity baseEntity)
                    {
                        baseEntity.ModifiedDate = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}