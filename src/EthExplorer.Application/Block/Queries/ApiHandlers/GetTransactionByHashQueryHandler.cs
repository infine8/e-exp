using EthExplorer.ApiContracts.Transaction;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetTransactionByHashQueryHandler : BaseHandler, IQueryHandler<GetTransactionByHashQuery, GetTransactionResponse>
{
    private readonly IBlockRepository _blockRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IContractRepository _contractRepository;
    
    public GetTransactionByHashQueryHandler(IServiceProvider sp, ITransactionRepository transactionRepository, IContractRepository contractRepository, IBlockRepository blockRepository) : base(sp)
    {
        _transactionRepository = transactionRepository;
        _contractRepository = contractRepository;
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetTransactionResponse> Handle(GetTransactionByHashQuery query, CancellationToken cancellationToken)
    {
        var txHash = new TransactionHash(query.TxHash);

        var summary = await _transactionRepository.FindTransaction(txHash);
        if (summary is null) throw new DomainException($"Transaction {txHash} not found");

        var lastBlockNum = await _blockRepository.GetLastBlockNumber();

        var summaryView = Map<DetailedTransactionView>(summary);
        summaryView.Confirmations = lastBlockNum - summaryView.BlockNumber;

        var internalTxs = await _transactionRepository.FindInternalTransactions(txHash);
        
        var tokenTransfers = await _transactionRepository.FindTokenTransfers(txHash);
        var contracts = await _contractRepository.FindContracts(tokenTransfers.Select(_ => new ContractAddress(_.ContractAddress)).Distinct());

        var tokenTransferViews = tokenTransfers.Select(Map<TransactionTokenTransferView>).ToList();

        foreach (var item in tokenTransferViews)
        {
            var contract = contracts.FirstOrDefault(_ => _.Address == item.Contract.ContractAddress);
            item.Contract.LogoUrl = contract?.LogoUrl;
        }
        
        return new GetTransactionResponse(
            summaryView,
            tokenTransferViews, 
            internalTxs.Select(Map<TransactionInternalTxView>)
        );
    }
}