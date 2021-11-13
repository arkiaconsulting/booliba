// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Customers;
using Booliba.QuerySide;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booliba.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            IMediator mediator,
            ILogger<CustomersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddCustomer(
            [FromRoute] Guid id,
            AddCustomerRequest request,
            CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope(new Dictionary<string, string> { { "CustomerId", id.ToString() }, { "Endpoint", nameof(AddCustomer) }, { "Side", "Command" } });

            await _mediator.Send(
                new AddCustomerCommand(id, request.Name),
                cancellationToken
            );

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveCustomer(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope(new Dictionary<string, string> { { "CustomerId", id.ToString() }, { "Endpoint", nameof(RemoveCustomer) }, { "Side", "Command" } });

            await _mediator.Send(
                new RemoveCustomerCommand(id),
                cancellationToken
            );

            return Ok();
        }

        [HttpGet()]
        public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope(new Dictionary<string, string> { { "Endpoint", nameof(GetCustomers) }, { "Side", "Query" } });

            var response = await _mediator.Send(
                new GetCustomersQuery(),
                cancellationToken
            );

            return Ok(
                response?.Results.Select(wr =>
                    new CustomerResponse(
                        wr.Id,
                        wr.Name)
                )
            );
        }
    }

    public record AddCustomerRequest(string Name);
    public record CustomerResponse(Guid Id, string Name);

}
