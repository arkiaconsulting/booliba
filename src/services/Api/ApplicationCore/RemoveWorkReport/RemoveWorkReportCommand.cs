// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.RemoveWorkReport
{
    public record RemoveWorkReportCommand(Guid WorkReportId) : IRequest;

    internal class RemoveDaysCommandHandler : IRequestHandler<RemoveWorkReportCommand>
    {
        private readonly IEventStore _eventStore;

        public RemoveDaysCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        async Task<Unit> IRequestHandler<RemoveWorkReportCommand, Unit>.Handle(RemoveWorkReportCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (events.OfType<ReportAdded>().Any())
            {
                await _eventStore.Save(new WorkReportRemoved(request.WorkReportId), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
