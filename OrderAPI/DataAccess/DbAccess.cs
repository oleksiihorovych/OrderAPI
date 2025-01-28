using Microsoft.EntityFrameworkCore;
using OrderAPI.DataAccess.Models;

namespace OrderAPI.DataAccess
{
    public class DbAccess : DbContext
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public DbAccess(DbContextOptions<DbAccess> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);

            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

