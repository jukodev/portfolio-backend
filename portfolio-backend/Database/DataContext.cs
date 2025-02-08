using Microsoft.EntityFrameworkCore;
using portfolio_backend.Database.Models;

namespace portfolio_backend.Database
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public required DbSet<DepotEntity> Depots { get; set; }
        public required DbSet<WebScrapEntity> WebScraps { get; set; }
        public required DbSet<StockEntity> Stocks { get; set; }
        public required DbSet<TransactionEntity> StockValues { get; set; }
    }
}
