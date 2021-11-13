// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.Customers
{
    public record RemoveCustomerCommand(Guid CustomerId) : IRequest;

    internal class RemoveCustomerCommandHandler : IRequestHandler<RemoveCustomerCommand>
    {
        private readonly IEventStore _eventStore;

        public RemoveCustomerCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<RemoveCustomerCommand, Unit>.Handle(RemoveCustomerCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.CustomerId, cancellationToken);
            if (!events.Any())
            {
                return Unit.Value;
            }

            var aggregate = AggregateRoot.ReHydrate<CustomerAggregate>(request.CustomerId, events);
            aggregate.Remove();

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }

    public record CustomerRemoved(Guid CustomerId) : DomainEvent(CustomerId);
}
