using System.ComponentModel.DataAnnotations.Schema;

namespace EthExplorer.Infrastructure.Common;

public abstract record DbBaseModel
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("timestamp")]
    public ulong Timestamp { get; set; }
}