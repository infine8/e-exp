using EthExplorer.ApiContracts.Address.Queries;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.Api.Controllers;

public class AddressController : BaseController
{
    public AddressController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<GetAddressSummaryInfoResponse> GetAddressSummaryInfo([FromQuery] GetAddressSummaryInfoQuery request, CancellationToken cancellationToken) => await SendQuery(request, cancellationToken);
    
    [HttpGet]
    public async Task<GetAddressTokenTransfersResponse> GetAddressTokenTransfersByTokenType([FromQuery] GetAddressTokenTransfersByTokenTypeQuery request, CancellationToken cancellationToken) => await SendQuery(request, cancellationToken);
    
    [HttpGet]
    public async Task<GetAddressTokenTransfersResponse> GetAddressTokenTransfersByContractAddress([FromQuery] GetAddressTokenTransfersByContractAddressQuery request, CancellationToken cancellationToken) => await SendQuery(request, cancellationToken);
}