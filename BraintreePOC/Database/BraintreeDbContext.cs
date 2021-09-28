using BraintreePOC.Database.Entities;
using Microsoft.EntityFrameworkCore;

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
    }
}
