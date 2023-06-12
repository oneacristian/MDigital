using MDigital.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MDigital
{
    public class MDigitalDBContext : DbContext
    {
        public MDigitalDBContext(DbContextOptions<MDigitalDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Payment>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Transaction>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<TransactionArticle>()
                .HasKey(c => c.Id);

            // Foreign Keys
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payment)
                .WithMany()
                .HasForeignKey(t => t.PaymentId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TransactionArticle> TransactionArticles { get; set; }
    }
}
