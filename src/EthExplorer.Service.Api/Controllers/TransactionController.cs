using EthExplorer.ApiContracts.Transaction;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.Api.Controllers;

public class TransactionController : BaseController
{
    public TransactionController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<GetLastTransactionsResponse> GetLastTransactions([FromQuery] GetLastTransactionsQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);
    
    [HttpGet]
    public async Task<GetTransactionResponse> GetTransactionByHash([FromQuery] GetTransactionByHashQuery request, CancellationToken cancellationToken) => await SendQuery(request, cancellationToken);
}