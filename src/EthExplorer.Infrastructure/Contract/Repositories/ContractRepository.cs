using EthExplorer.Application.Contract.Queries.Web3;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ValueObjects;
using EthExplorer.Domain.Contract.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Contract.Repositories;

public class ContractRepository : BaseRepository<IContractRepository>, IContractRepository
{
    private readonly ClickHouseDbReaderContext _dbContext;
    private readonly IMediator _mediator;

    public ContractRepository(IServiceProvider sp) : base(sp)
    {
        _dbContext = sp.GetRequiredService<ClickHouseDbReaderContext>();
        _mediator = sp.GetRequiredService<IMediator>();
    }

    [Cache, Diagnostic]
    public virtual async Task<ContractViewModel?> GetContract(Guid id)
    {
        var model = await _dbContext.Contracts.FirstOrDefaultAsync(_ => _.Id == id);

        return await MapToContractViewModel(model);
    }

    [Cache, Diagnostic]
    public virtual async Task<ContractViewModel?> FindContract(ContractAddress contractAddress)
    {
        var items = await FindContracts(new[] { contractAddress });

        return items.FirstOrDefault();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractViewModel>> FindContracts(IEnumerable<ContractAddress> contractAddresses)
    {
        if (!contractAddresses.HasItems()) return Array.Empty<ContractViewModel>();

        var addresses = contractAddresses.Select(_ => _.Value);

        var sql = $@"SELECT * FROM `contract` WHERE `id` IN (SELECT `id` FROM `contract_address_id` WHERE `address` IN ({string.Join(',', addresses.Select(_ => $"'{_}'"))}))";

        var models = await _dbContext.Contracts.FromSqlRaw(sql).ToListAsync();

        var items = new List<ContractViewModel>();
        foreach (var model in models)
        {
            items.Add(await MapToContractViewModel(model));
        }

        return items;
    }

    [Cache, Diagnostic]
    public virtual async Task<ContractType?> GetContractType(ContractAddress address)
    {
        var transferTx = await _dbContext.TransactionTokenTransfers.FirstOrDefaultAsync(_ => _.ContractAddress == address.Value);
        return (ContractType?)transferTx?.ContractType;
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetTotalHolders(ContractAddress address)
    {
        var sql = $@"SELECT COUNT() FROM token_holder_view WHERE contract_address = '{address.Value}' AND `value` != '{DbBlockBalanceReadModel.DEFAULT_VALUE}'";
        
        return Convert.ToUInt64(await _dbContext.RawSqlQueryFirst(sql));
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractHolderViewModel>> GetContractHolders(ContractAddress address, int skip, int limit)
    {
        var totalSupply = await GetTokenTotalSupply(address);

        var valueFilter = $"`value` != '{DbBlockBalanceReadModel.DEFAULT_VALUE}'";
        var orderByCondition = $"leftPad(substring(`value`, 1, if(dp = 0, length(`value`) + 1, dp) - 1), 100, '0') DESC, `value` DESC";

        var sql = $@"SELECT address, contract_address, `value`, position(`value`, '.') dp FROM token_holder_view 
                        WHERE contract_address = '{address.Value}' AND {valueFilter} 
                        ORDER BY {orderByCondition} LIMIT {skip},{limit}";


        var items = await _dbContext.RawSqlQuery(sql, reader => new ContractHolderViewModel(
            AddressValue.GetValueOrNull(reader["address"]),
            reader["value"].ToString(),
            totalSupply > 0 && decimal.TryParse(reader["value"].ToString(), out var value) ? (value / totalSupply.Value) : null)
        );

        return items;
    }

    [Cache, Diagnostic]
    public virtual async Task<decimal?> GetTotalHeldAmount(ContractAddress address)
    {
        var sumValue = "SUM(toFloat64OrNull(`value`)) as `value`";

        var sql = $@"SELECT {sumValue} FROM token_holder_view WHERE contract_address = '{address.Value}'";
        
        var val = await _dbContext.RawSqlQueryFirst(sql);

        return val is null ? null : Convert.ToDecimal(val);
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetTotalTransferCount(ContractAddress address, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = @$"SELECT COUNT() FROM tx_token_transfer_ca_id WHERE contract_address = '{address.Value}' AND {blockNumSqlCond}";

        var count = await _dbContext.RawSqlQueryFirst(sql);

        return Convert.ToUInt64(count);
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractTransferViewModel>> GetContractTransfers(ContractAddress address, int skip, int limit, BlockNumber? blockNumber = null)
    {
        var blockNumSqlCond = (blockNumber is not null) ? $"block_num < {blockNumber.Value}" : "1=1";

        var sql = @$"SELECT tt.* FROM tx_token_transfer tt 
                    WHERE
                    tt.id IN (
                            SELECT id FROM tx_token_transfer_ca_id WHERE contract_address = '{address.Value}' AND {blockNumSqlCond}
                            ORDER BY block_timestamp desc, index desc LIMIT {skip},{limit}
                        )
                    ";

        var items = await _dbContext.TransactionTokenTransfers.FromSqlRaw(sql).ToListAsync();

        return items.Select(Map<ContractTransferViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetLastTxsCount(ContractAddress address, DateTime sinceDateUtc)
    {
        var sql = $@"
            SELECT COUNT() FROM tx_token_transfer_ca_id t1 
            WHERE contract_address = '{address.Value}' AND block_timestamp >= {sinceDateUtc.ToUnixTimestamp()}
        ";

        return Convert.ToUInt64(await _dbContext.RawSqlQueryFirst(sql));
    }


    [Cache(CachePeriod.UltraLong), Diagnostic]
    protected virtual async Task<string?> GetTokenName(ContractAddress address)
        => await _mediator.Send(new GetTokenNameQuery(address));

    [Cache(CachePeriod.UltraLong), Diagnostic]
    protected virtual async Task<string?> GetTokenSymbol(ContractAddress address)
        => await _mediator.Send(new GetTokenSymbolQuery(address));

    [Cache(CachePeriod.UltraLong), Diagnostic]
    protected virtual async Task<ulong?> GetTokenTotalSupply(ContractAddress address)
        => await _mediator.Send(new GetTokenTotalSupplyQuery(address));

    [Cache(CachePeriod.UltraLong), Diagnostic]
    protected virtual async Task<byte?> GetTokenDecimals(ContractAddress address)
        => await _mediator.Send(new GetTokenDecimalsQuery(address));

    private async Task<ContractViewModel?> MapToContractViewModel(DbContractReadModel? model)
    {
        if (model is null) return null;

        model.Name = await GetTokenName(new ContractAddress(model.Address)) ?? default;
        model.Symbol = await GetTokenSymbol(new ContractAddress(model.Address)) ?? default;
        model.Decimals = await GetTokenDecimals(new ContractAddress(model.Address)) ?? default;
        model.TotalSupply = await GetTokenTotalSupply(new ContractAddress(model.Address)) ?? default;

        return Map<ContractViewModel>(model);
    }
}