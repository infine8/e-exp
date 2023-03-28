using EthExplorer.ApiContracts.Transaction;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetLastTransactionsQueryHandler : BaseHandler, IQueryHandler<GetLastTransactionsQuery, GetLastTransactionsResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    
    public GetLastTransactionsQueryHandler(IServiceProvider sp, ITransactionRepository transactionRepository) : base(sp)
    {
        _transactionRepository = transactionRepository;
    }

    public async ValueTask<GetLastTransactionsResponse> Handle(GetLastTransactionsQuery query, CancellationToken cancellationToken)
    {
        var items = await _transactionRepository.GetLastTransactions(query.Limit);

        return new GetLastTransactionsResponse { Items = items.Select(Map<LastTransactionPreview>), TotalCount = (ulong)items.Count };
    }
}