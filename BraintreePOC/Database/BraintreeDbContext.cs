using BraintreePOC.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BraintreePOC.Entities
{

    public class BraintreeDbContext : DbContext
    {
        public BraintreeDbContext(DbContextOptions<BraintreeDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerCreditCard> CreditCards { get; set; }
        public DbSet<CustomerAddress> Addresses { get; set; }
        public DbSet<CustomerTransaction> Transactions { get; set; }
        public DbSet<CustomerTransactionRefund> TransactionRefunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerTransaction>().Property(x => x.Amount).HasColumnType("DECIMAL(10,2)");
            modelBuilder.Entity<CustomerTransactionRefund>().Property(x => x.Amount).HasColumnType("DECIMAL(10,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
