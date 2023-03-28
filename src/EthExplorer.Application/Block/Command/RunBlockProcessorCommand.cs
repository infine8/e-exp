using System.Numerics;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract;
using Nethereum.RPC.Eth.DTOs;

namespace EthExplorer.Application.Block.Command;

public record RunBlockProcessorCommand(
    BlockNumber BlockNumber,
    Action<BlockWithTransactions> ProcessBlockFunc,
    Func<TransactionReceiptVO, Task> ProcessTransactionFunc,
    Func<ContractCreationVO, Task> ProcessContractCreationFunc,
    Func<string, string, ContractType, string, string, BigInteger, Task> ProcessTokenTransfersFunc
    ) : ICommand;