// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Customers;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record AddWorkReportCommand(Guid WorkReportId, string Name, IEnumerable<DateOnly> Days, Guid? CustomerId) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddWorkReportCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly CustomerService _customerService;

        public AddWorkReportCommandHandler(
            IEventStore repository,
            CustomerService customerService
            )
        {
            _eventStore = repository;
            _customerService = customerService;
        }

        async Task<Unit> IRequestHandler<AddWorkReportCommand, Unit>.Handle(AddWorkReportCommand request, CancellationToken cancellationToken)
        {
            if (request.CustomerId.HasValue)
            {
                var customerExists = await _customerService.Exists(request.CustomerId.Value);

                if (!customerExists)
                {
                    throw new CustomerNotFoundException();
                }
            }

            var aggregate = WorkReportAggregate.Create(request.WorkReportId, request.Name, request.Days, request.CustomerId);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }
    public record ReportAdded(Guid AggregateId, string WorkReportName, IEnumerable<DateOnly> Days, Guid? CustomerId) : DomainEvent(AggregateId);
}