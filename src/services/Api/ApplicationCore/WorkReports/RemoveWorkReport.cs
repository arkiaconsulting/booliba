﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.WorkReports
{
    public record RemoveWorkReportCommand(Guid WorkReportId) : IRequest;

    internal class RemoveWorkReportCommandHandler : IRequestHandler<RemoveWorkReportCommand>
    {
        private readonly IEventStore _eventStore;

        public RemoveWorkReportCommandHandler(IEventStore eventStore) => _eventStore = eventStore;

        async Task<Unit> IRequestHandler<RemoveWorkReportCommand, Unit>.Handle(RemoveWorkReportCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (!events.Any())
            {
                return Unit.Value;
            }
            var aggregate = AggregateRoot.ReHydrate<WorkReportAggregate>(request.WorkReportId, events);
            aggregate.Remove();

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }

    public record WorkReportRemoved(Guid AggregateId) : DomainEvent(AggregateId);
}
