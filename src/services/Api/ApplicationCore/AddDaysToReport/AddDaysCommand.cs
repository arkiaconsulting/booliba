// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.AddDaysToReport
{
    public record AddDaysCommand(Guid WorkReportId, IEnumerable<DateOnly> DaysToAdd) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddDaysCommand>
    {
        private readonly IEventStore _eventStore;

        public AddWorkReportCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<AddDaysCommand, Unit>.Handle(AddDaysCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (!events.Any())
            {
                throw new WorkReportNotFoundException(request.WorkReportId);
            }

            var aggregate = WorkReportAggregate.ReHydrate(request.WorkReportId, events);
            aggregate.AddDays(request.DaysToAdd);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }
}
