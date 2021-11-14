// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record RemoveDaysCommand(Guid WorkReportId, IEnumerable<DateOnly> DaysToRemove) : IRequest;

    internal class RemoveDaysCommandHandler : IRequestHandler<RemoveDaysCommand>
    {
        private readonly IEventStore _eventStore;

        public RemoveDaysCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<RemoveDaysCommand, Unit>.Handle(RemoveDaysCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (!events.Any())
            {
                throw new AggregateNotFound(request.WorkReportId);
            }

            var aggregate = AggregateRoot.ReHydrate<WorkReportAggregate>(request.WorkReportId, events);
            aggregate.RemoveDays(request.DaysToRemove);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }

    public record DaysRemoved(Guid AggregateId, IEnumerable<DateOnly> Days) : DomainEvent(AggregateId);

}
