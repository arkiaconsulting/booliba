// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Customers;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record SetWorkReportCustomerCommand(Guid WorkReportId, Guid? CustomerId) : IRequest;

    internal class SetWorkReportCustomerCommandHandler : IRequestHandler<SetWorkReportCustomerCommand>
    {
        private readonly CustomerService _customerService;
        private readonly IEventStore _eventStore;

        public SetWorkReportCustomerCommandHandler(
            CustomerService customerService,
            IEventStore eventStore
            )
        {
            _customerService = customerService;
            _eventStore = eventStore;
        }

        async Task<Unit> IRequestHandler<SetWorkReportCustomerCommand, Unit>.Handle(SetWorkReportCustomerCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            var aggregate = AggregateRoot.ReHydrate<WorkReportAggregate>(request.WorkReportId, events);

            if (request.CustomerId.HasValue)
            {
                var customerExist = await _customerService.Exists(request.CustomerId.Value, cancellationToken);

                if (!customerExist)
                {
                    throw new CustomerNotFoundException();
                }

                aggregate.SetCustomer(request.CustomerId!.Value);
            }
            else
            {
                aggregate.UnsetCustomer();
            }

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }

    public record WorkReportCustomerUnset(Guid AggregateId) : DomainEvent(AggregateId);
    public record WorkReportCustomerSet(Guid AggregateId, Guid CustomerId) : DomainEvent(AggregateId);
}
