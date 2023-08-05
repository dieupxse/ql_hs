using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Remotion.Linq.Parsing.ExpressionVisitors;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QL_HS.Database
{
    public class QLHSDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public QLHSDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<GuardianEntity> Guardians { get; set; }
        public DbSet<PickupEntity> Pickups { get; set; }
        public DbSet<AccountEntity> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentEntity>(b =>
            {
                b.HasKey(e => e.Id);
                b.HasIndex(e => new { e.Name, e.Class });
                b.HasOne(e => e.Guardian).
                WithMany()
                .HasForeignKey(e => e.GuardianId);
            });

            modelBuilder.Entity<PickupEntity>(b =>
            {
                b.HasKey(e => e.Id);
                b.HasIndex(e => new { e.StudentId, e.GuardianId });
                b.HasOne(e=>e.Student).WithMany().HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.ClientSetNull);
                b.HasOne(e => e.Guardian).WithMany().HasForeignKey(e => e.GuardianId).OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<GuardianEntity>(b =>
            {
                b.HasKey(e => e.Id);
                b.HasOne(e => e.Account).WithOne(e => e.Guardian).HasForeignKey<GuardianEntity>(e => e.AccountId);
            });

            modelBuilder.Entity<AccountEntity>(b =>
            {
                b.HasKey(e => e.Id);
            });


            var configureSoftDelMethod = GetType().GetTypeInfo().DeclaredMethods.Single(m => m.Name == nameof(ConfigureSoftDeleteFilter));
            var args = new object[] { modelBuilder };
            var tenantEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType));
            foreach (var entityType in tenantEntityTypes)
                configureSoftDelMethod.MakeGenericMethod(entityType.ClrType).Invoke(this, args);
        }

        void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => e.Status == EntityStatus.ENABLE);
        }
    }

}
