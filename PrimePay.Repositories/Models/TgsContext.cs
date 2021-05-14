
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using Tgs.Payment.PrimePay.Repositories.Entities.Configurations;
namespace Tgs.Payment.PrimePay.Repositories.Entities
{
    public partial class TgsContext : DbContext
    {
        public virtual DbSet<DepositForm> DepositForms { get; set; }
        public virtual DbSet<WithdrawForm> WithdrawForms { get; set; }

        public TgsContext(DbContextOptions<TgsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_trgm")
                .HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.ApplyConfiguration(new Configurations.DepositFormConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.WithdrawFormConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
