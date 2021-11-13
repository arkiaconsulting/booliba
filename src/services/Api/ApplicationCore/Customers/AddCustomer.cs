// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.Customers
{
    public record AddCustomerCommand(Guid CustomerId, string Name) : IRequest;

    internal class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand>
    {
        private readonly IEventStore _eventStore;

        public AddCustomerCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<AddCustomerCommand, Unit>.Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            var aggregate = CustomerAggregate.Create(request.CustomerId, request.Name);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }

    public record CustomerAdded(Guid CustomerId, string Name) : DomainEvent(CustomerId);
}
