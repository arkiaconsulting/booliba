// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;
using Microsoft.Extensions.Logging;

namespace Booliba.QuerySide
{
    public record GetCustomersQuery : IRequest<GetCustomersResponse>;
    public record GetCustomersResponse(CustomerEntity[] Results);

    internal class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, GetCustomersResponse>
    {
        private readonly ICustomerProjection _customerProjection;
        private readonly ILogger<GetCustomersQueryHandler> _logger;

        public GetCustomersQueryHandler(
            ICustomerProjection customerProjection,
            ILogger<GetCustomersQueryHandler> logger)
        {
            _customerProjection = customerProjection;
            _logger = logger;
        }

        async Task<GetCustomersResponse> IRequestHandler<GetCustomersQuery, GetCustomersResponse>.Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling a query {QueryType}", request.GetType().Name);

            var entities = await _customerProjection.List(cancellationToken);

            return new GetCustomersResponse(entities);
        }
    }
}
