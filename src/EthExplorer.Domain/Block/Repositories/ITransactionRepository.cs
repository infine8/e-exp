using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Domain.Block.Repositories;

public interface ITransactionRepository
{

    Task<IReadOnlyList<TransactionViewModel>> GetLastTransactions(int limit);
    Task<TransactionViewModel?> FindTransaction(TransactionHash transactionHash);
    Task<IReadOnlyList<TransactionTraceViewModel>> FindInternalTransactions(TransactionHash transactionHash);
    Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(TransactionHash transactionHash);
    Task<ulong> GetTotalTxCount();
}