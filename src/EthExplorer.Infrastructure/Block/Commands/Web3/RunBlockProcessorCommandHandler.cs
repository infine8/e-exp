using EthExplorer.Application.Block.Command;
using EthExplorer.Application.Common;
using EthExplorer.Infrastructure.Block.LogEvents;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Infrastructure.Block.Commands.Web3;


public class RunBlockProcessorCommandHandler : BaseHandler, ICommandHandler<RunBlockProcessorCommand>
{
    private readonly IWeb3 _web3;

    public RunBlockProcessorCommandHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<Unit> Handle(RunBlockProcessorCommand request, CancellationToken cancellationToken)
    {
        var blockProcessor = _web3.Processing.Blocks.CreateBlockProcessor(steps =>
        {
            steps.BlockStep.AddSynchronousProcessorHandler(request.ProcessBlockFunc);
            steps.TransactionReceiptStep.AddProcessorHandler(request.ProcessTransactionFunc);
            steps.ContractCreationStep.AddProcessorHandler(request.ProcessContractCreationFunc);
        }, log: LogService.Logger);

        var logProcessor = _web3.Processing.Logs.CreateProcessor(new ProcessorHandler<FilterLog>[]
        {
            new EventLogProcessorHandler<TransferEventERC20>(log => request.ProcessTokenTransfersFunc(log.Log.TransactionHash, log.Log.Address, log.Event.ContractType, log.Event.From, log.Event.To, log.Event.Value)),
            new EventLogProcessorHandler<TransferEventERC721>(log => request.ProcessTokenTransfersFunc(log.Log.TransactionHash, log.Log.Address, log.Event.ContractType, log.Event.From, log.Event.To, log.Event.Value)),
            new EventLogProcessorHandler<TransferSingleEventERC1155>(log => request.ProcessTokenTransfersFunc(log.Log.TransactionHash, log.Log.Address, log.Event.ContractType, log.Event.From, log.Event.To, log.Event.Value)),
        }, log: LogService.Logger);

        await logProcessor.ExecuteAsync(request.BlockNumber.Value, cancellationToken, request.BlockNumber.Value);
        await blockProcessor.ExecuteAsync(request.BlockNumber.Value, cancellationToken, request.BlockNumber.Value);
        
        return Unit.Value;
    }
}