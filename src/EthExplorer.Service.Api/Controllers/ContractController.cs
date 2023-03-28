using EthExplorer.ApiContracts.Contract.Queries;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.Api.Controllers;

public class ContractController : BaseController
{
    public ContractController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<GetContractResponse> GetContractByAddress([FromQuery] GetContractByAddressQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);
    
    [HttpGet]
    public async Task<GetContractHoldersResponse> GetContractHolders([FromQuery] GetContractHoldersQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);
    
    [HttpGet]
    public async Task<GetContractTransfersResponse> GetContractTransfers([FromQuery] GetContractTransfersQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);
}