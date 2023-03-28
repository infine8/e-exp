using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Domain.Contract.Repositories;

public interface IContractRepository
{
    Task<ContractViewModel?> GetContract(Guid id);
    Task<ContractViewModel?> FindContract(ContractAddress contractAddress);
    Task<IReadOnlyList<ContractViewModel>> FindContracts(IEnumerable<ContractAddress> contractAddresses);
    Task<ContractType?> GetContractType(ContractAddress address);
    Task<ulong> GetTotalHolders(ContractAddress address);
    Task<IReadOnlyList<ContractHolderViewModel>> GetContractHolders(ContractAddress address, int skip, int limit);
    Task<decimal?> GetTotalHeldAmount(ContractAddress address);
    Task<ulong> GetTotalTransferCount(ContractAddress address, BlockNumber? blockNumber = null);
    Task<IReadOnlyList<ContractTransferViewModel>> GetContractTransfers(ContractAddress address, int skip, int limit, BlockNumber? blockNumber = null);
    Task<ulong> GetLastTxsCount(ContractAddress address, DateTime sinceDateUtc);
}