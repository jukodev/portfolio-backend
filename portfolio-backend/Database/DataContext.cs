using Microsoft.EntityFrameworkCore;
using portfolio_backend.Database.Models;

namespace portfolio_backend.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<DepotEntity> Depots { get; set; }
        public DbSet<WebScrapEntity> WebScraps { get; set; }
        public DbSet<StockEntity> Stocks { get; set; }
        public DbSet<TransactionEntity> StockValues { get; set; }
    }
}
