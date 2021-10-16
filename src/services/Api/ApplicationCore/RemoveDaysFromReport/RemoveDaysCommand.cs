// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.RemoveDaysFromReport
{
    public record RemoveDaysCommand(Guid WorkReportId, IEnumerable<DateOnly> DaysToRemove) : IRequest;

    internal class RemoveDaysCommandHandler : IRequestHandler<RemoveDaysCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IRepository _repository;

        public RemoveDaysCommandHandler(IEventBus eventBus, IRepository repository)
        {
            _eventBus = eventBus;
            _repository = repository;
        }

        async Task<Unit> IRequestHandler<RemoveDaysCommand, Unit>.Handle(RemoveDaysCommand request, CancellationToken cancellationToken)
        {
            var events = await _repository.Load(request.WorkReportId, cancellationToken);
            var reportAddedEvent = events.OfType<ReportAdded>().Where(e => e.WorkReportId == request.WorkReportId).Single();
            var daysAddedEvents = events.OfType<DaysAdded>().Where(e => e.WorkReportId == request.WorkReportId);

            var daysToRemoveEffectively = request.DaysToRemove.Intersect(reportAddedEvent.Days.Concat(daysAddedEvents.SelectMany(e => e.Days)));

            if (daysToRemoveEffectively.Any())
            {
                await _eventBus.Publish(new DaysRemoved(request.WorkReportId, request.DaysToRemove), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
