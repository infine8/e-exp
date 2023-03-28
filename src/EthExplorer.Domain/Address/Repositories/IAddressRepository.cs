using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Address.ViewModels;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.ValueObjects;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Domain.Address.Repositories;

public interface IAddressRepository
{
    Task<IReadOnlyList<AddressBalanceViewModel>> GetAddressBalances(AddressValue address, IEnumerable<ContractAddress?>? contractAddresses = null);
    Task<IReadOnlyList<AddressBalanceViewModel>> GetAllAddressBalances(AddressValue address);
    Task<AddressBalanceViewModel> GetAddressBalance(AddressValue address, ContractAddress? contractAddress = null);
    Task<BlockBalanceViewModel?> FindAddressFirstBalanceChange(AddressValue address);
    Task<BlockBalanceViewModel?> FindAddressLastBalanceChange(AddressValue address, BlockNumber? blockNumber = null);
    Task<ulong> GetTotalTxCount(AddressValue address, BlockNumber? blockNumber = null);
    Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(AddressValue address, ContractType contractType, int skip, int limit, BlockNumber? blockNumber = null);
    Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(AddressValue address, ContractAddress contractAddress, int skip, int limit, BlockNumber? blockNumber = null);
    Task<ulong> GetTokenTransfersTotalCount(AddressValue address, ContractType contractType, BlockNumber? blockNumber = null);
    Task<ulong> GetTokenTransfersTotalCount(AddressValue address, ContractAddress contractAddress, BlockNumber? blockNumber = null);
}