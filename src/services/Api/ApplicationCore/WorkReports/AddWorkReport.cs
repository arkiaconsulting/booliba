// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record AddWorkReportCommand(Guid WorkReportId, string Name, IEnumerable<DateOnly> Days) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddWorkReportCommand>
    {
        private readonly IEventStore _eventStore;

        public AddWorkReportCommandHandler(IEventStore repository) => _eventStore = repository;

        async Task<Unit> IRequestHandler<AddWorkReportCommand, Unit>.Handle(AddWorkReportCommand request, CancellationToken cancellationToken)
        {
            var aggregate = WorkReportAggregate.Create(request.WorkReportId, request.Name, request.Days);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }
    public record ReportAdded(Guid AggregateId, string WorkReportName, IEnumerable<DateOnly> Days) : DomainEvent(AggregateId);
}