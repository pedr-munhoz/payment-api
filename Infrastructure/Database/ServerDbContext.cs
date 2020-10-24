using Microsoft.EntityFrameworkCore;
using payment_api.Models;

namespace payment_api.Infrastructure.Database
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PaymentEntity> Payments { get; set; }
    }
}