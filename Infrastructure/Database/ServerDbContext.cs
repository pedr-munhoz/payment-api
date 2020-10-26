using Microsoft.EntityFrameworkCore;
using payment_api.Models;

namespace payment_api.Infrastructure.Database
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PaymentEntity> Payments { get; set; }

        public DbSet<PaymentInstallmentEntity> Installments { get; set; }

        public DbSet<AntecipationEntity> Antecipations { get; set; }

        public DbSet<AntecipationAnalysis> AntecipationAnalyses { get; set; }
    }
}