using EthExplorer.Infrastructure.Block.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EthExplorer.Infrastructure;

public sealed class ClickHouseDbReaderContext : DbContext
{
    public DbSet<DbBlockReadModel> Blocks { get; set; }
    public DbSet<DbTransactionReadModel> Transactions { get; set; }
    public DbSet<DbTransactionTokenTransferReadModel> TransactionTokenTransfers { get; set; }
    public DbSet<DbTransactionInternalReadModel> InternalTransactions { get; set; }
    public DbSet<DbBlockBalanceReadModel> BlockBalanceChanges { get; set; }
    public DbSet<DbContractReadModel> Contracts { get; set; }
    
    public ClickHouseDbReaderContext(DbContextOptions<ClickHouseDbReaderContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(36, 18);
    }
}