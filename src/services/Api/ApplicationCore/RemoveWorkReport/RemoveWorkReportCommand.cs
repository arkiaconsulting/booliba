// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.RemoveWorkReport
{
    public record RemoveWorkReportCommand(Guid WorkReportId) : IRequest;

    internal class RemoveDaysCommandHandler : IRequestHandler<RemoveWorkReportCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IRepository _repository;

        public RemoveDaysCommandHandler(IEventBus eventBus, IRepository repository)
        {
            _eventBus = eventBus;
            _repository = repository;
        }

        async Task<Unit> IRequestHandler<RemoveWorkReportCommand, Unit>.Handle(RemoveWorkReportCommand request, CancellationToken cancellationToken)
        {
            var events = await _repository.Load(request.WorkReportId, cancellationToken);
            if (events.OfType<ReportAdded>().Where(e => e.WorkReportId == request.WorkReportId).Any())
            {
                await _eventBus.Publish(new WorkReportRemoved(request.WorkReportId), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
