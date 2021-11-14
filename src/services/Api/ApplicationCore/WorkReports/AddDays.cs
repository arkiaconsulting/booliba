// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record AddDaysCommand(Guid WorkReportId, IEnumerable<DateOnly> DaysToAdd) : IRequest;

    internal class AddDaysCommandHandler : IRequestHandler<AddDaysCommand>
    {
        private readonly IEventStore _eventStore;

        public AddDaysCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<AddDaysCommand, Unit>.Handle(AddDaysCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (!events.Any())
            {
                throw new AggregateNotFound(request.WorkReportId);
            }

            var aggregate = AggregateRoot.ReHydrate<WorkReportAggregate>(request.WorkReportId, events);
            aggregate.AddDays(request.DaysToAdd);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }
    public record DaysAdded(Guid AggregateId, IEnumerable<DateOnly> Days) : DomainEvent(AggregateId);
}
