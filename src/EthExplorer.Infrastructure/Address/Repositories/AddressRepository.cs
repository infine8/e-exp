using EthExplorer.Domain.Address.Repositories;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Address.ViewModels;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.ValueObjects;
using EthExplorer.Domain.Contract.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Address.Repositories;

public class AddressRepository : BaseRepository<IAddressRepository>, IAddressRepository
{
    private readonly ClickHouseDbReaderContext _dbContext;

    public AddressRepository(IServiceProvider sp) : base(sp)
    {
        _dbContext = sp.GetRequiredService<ClickHouseDbReaderContext>();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<AddressBalanceViewModel>> GetAddressBalances(AddressValue address, IEnumerable<ContractAddress?>? contractAddresses = null)
    {
        var contractAddressFilter = contractAddresses.HasItems() ? $"contract_address in ({string.Join(',', contractAddresses.Select(ca => ca is null ? $"'{AddressValue.DEFAULT_ADDRESS}'" : $"'{ca.Value}'"))})" : "1=1";
        var valueFilter = $"`value` != '{DbBlockBalanceReadModel.DEFAULT_VALUE}'";
        var orderByCondition = $"leftPad(substring(`value`, 1, if(dp = 0, length(`value`) + 1, dp) - 1), 100, '0') DESC, `value` DESC";

        var sql = $@"SELECT `contract_address`, `value`, position(`value`, '.') dp FROM address_last_balance_view WHERE address = '{address.Value}' AND {contractAddressFilter} AND {valueFilter} ORDER BY {orderByCondition}";

        var items = await _dbContext.RawSqlQuery(sql, reader => new AddressBalanceViewModel(
            reader["value"].ToString(),
            AddressValue.GetValueOrNull(reader["contract_address"])
        ));

        if (items.Any(_ => !string.IsNullOrWhiteSpace(_.ContractAddress)))
        {
            var contractInfoSql = $@"SELECT * FROM `contract` WHERE `id` IN (SELECT `id` FROM `contract_address_id` 
                                    WHERE `address` IN ({string.Join(',',
                                        items
                                            .Where(_ => !string.IsNullOrWhiteSpace(_.ContractAddress))
                                            .Select(_ => $"'{_.ContractAddress}'"))}))";

            var contracts = await _dbContext.Contracts.FromSqlRaw(contractInfoSql).ToListAsync();
        }

        return items;
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<AddressBalanceViewModel>> GetAllAddressBalances(AddressValue address)
        => await GetAddressBalances(address);

    [Cache, Diagnostic]
    public virtual async Task<AddressBalanceViewModel> GetAddressBalance(AddressValue address, ContractAddress? contractAddress = null)
    {
        var items = await GetAddressBalances(address, new[] { contractAddress });

        return Map<AddressBalanceViewModel?>(items.FirstOrDefault()) ?? new(default, contractAddress?.Value);
    }


    [Cache, Diagnostic]
    public virtual async Task<BlockBalanceViewModel?> FindAddressFirstBalanceChange(AddressValue address)
    {
        var sql = $@"SELECT * FROM address_first_balance_change_view WHERE address = '{address.Value}'";

        var item = await _dbContext.RawSqlQueryFirstOfDefault(sql, reader => new BlockBalanceViewModel
        (
            Convert.ToUInt64(reader["block_num"]),
            Convert.ToUInt64(reader["block_timestamp"]).FromUnixTimestamp(),
            reader["address"]?.ToString(),
            reader["value"].ToString(),
            AddressValue.GetValueOrNull(reader["contract_address"]),
            reader["tx_hash"]?.ToString()
        ));

        return item;
    }

    [Cache, Diagnostic]
    public virtual async Task<BlockBalanceViewModel?> FindAddressLastBalanceChange(AddressValue address, BlockNumber? blockNumber = null)
    {
        var sql = $@"SELECT * FROM address_last_balance_view WHERE address = '{address.Value}' ORDER BY block_num DESC LIMIT 1";

        var item = await _dbContext.RawSqlQueryFirstOfDefault(sql, reader => new BlockBalanceViewModel
        (
            Convert.ToUInt64(reader["block_num"]),
            Convert.ToUInt64(reader["block_timestamp"]).FromUnixTimestamp(),
            AddressValue.GetValueOrNull(reader["address"]),
            reader["value"].ToString(),
            AddressValue.GetValueOrNull(reader["contract_address"]),
            reader["tx_hash"]?.ToString()
        ));

        return item;
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetTotalTxCount(AddressValue address, BlockNumber? blockNumber = null)
    {
        if (blockNumber is null)
        {
            var sql = $@"SELECT SUM(total_tx_count) FROM address_last_balance_view WHERE address = '{address.Value}'";

            var totalTxCount = await _dbContext.RawSqlQueryFirstOfDefault(sql);

            return Convert.ToUInt64(totalTxCount);
        }

        var query = _dbContext.BlockBalanceChanges.Where(_ => _.Address == address.Value);
        if (blockNumber is not null) query = query.Where(_ => _.BlockNumber <= blockNumber.Value);

        return (ulong)await query.LongCountAsync();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(AddressValue address, ContractType contractType, int skip, int limit, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = @$"SELECT tt.* FROM tx_token_transfer tt 
                    WHERE
                    tt.id IN (
                            SELECT t.id FROM (
                                SELECT id, block_num, index FROM tx_token_transfer_a_from_id WHERE from_address = '{address.Value}' AND contract_type = {contractType.Id()} AND {blockNumSqlCond}
                                UNION DISTINCT
                                SELECT id, block_num, index FROM tx_token_transfer_a_to_id WHERE to_address = '{address.Value}' AND contract_type = {contractType.Id()} AND {blockNumSqlCond}
                            ) t ORDER BY t.block_num desc, t.index desc LIMIT {skip},{limit}
                        ) ORDER BY tt.block_num desc, tt.index desc
                    ";


        var items = await _dbContext.TransactionTokenTransfers.FromSqlRaw(sql).ToListAsync();

        return items.Select(Map<ContractTransferViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(AddressValue address, ContractAddress contractAddress, int skip, int limit, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = @$"SELECT tt.* FROM tx_token_transfer tt 
                    WHERE
                    tt.id IN (
                            SELECT t.id FROM (
                                SELECT id, block_num, index FROM tx_token_transfer_a_from_id WHERE from_address = '{address.Value}' AND contract_address = '{contractAddress.Value}' AND {blockNumSqlCond}
                                UNION DISTINCT
                                SELECT id, block_num, index FROM tx_token_transfer_a_to_id WHERE to_address = '{address.Value}' AND contract_address = '{contractAddress.Value}' AND {blockNumSqlCond}
                            ) t ORDER BY t.block_num desc, t.index desc LIMIT {skip},{limit}
                        ) ORDER BY tt.block_num desc, tt.index desc
                    ";

        var items = await _dbContext.TransactionTokenTransfers.FromSqlRaw(sql).ToListAsync();

        return items.Select(Map<ContractTransferViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetTokenTransfersTotalCount(AddressValue address, ContractType contractType, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = $@"SELECT COUNT(DISTINCT id) FROM (
                                SELECT id, block_num, index FROM tx_token_transfer_a_from_id WHERE from_address = '{address.Value}' AND contract_type = {contractType.Id()} AND {blockNumSqlCond}
                                UNION DISTINCT
                                SELECT id, block_num, index FROM tx_token_transfer_a_to_id WHERE to_address = '{address.Value}' AND contract_type = {contractType.Id()} AND {blockNumSqlCond}
                            )";

        var count = await _dbContext.RawSqlQueryFirst(sql);

        return Convert.ToUInt64(count);
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetTokenTransfersTotalCount(AddressValue address, ContractAddress contractAddress, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = $@"SELECT COUNT(DISTINCT id) FROM (
                                SELECT id, block_num, index FROM tx_token_transfer_a_from_id WHERE from_address = '{address.Value}' AND contract_address = '{contractAddress.Value}' AND {blockNumSqlCond}
                                UNION DISTINCT
                                SELECT id, block_num, index FROM tx_token_transfer_a_to_id WHERE to_address = '{address.Value}' AND contract_address = '{contractAddress.Value}' AND {blockNumSqlCond}
                            )";

        var count = await _dbContext.RawSqlQueryFirst(sql);

        return Convert.ToUInt64(count);
    }
}