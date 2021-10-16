// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.AddDaysToReport
{
    public record AddDaysCommand(Guid WorkReportId, IEnumerable<DateOnly> DaysToAdd) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddDaysCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IRepository _repository;

        public AddWorkReportCommandHandler(IEventBus eventBus, IRepository repository)
        {
            _eventBus = eventBus;
            _repository = repository;
        }

        async Task<Unit> IRequestHandler<AddDaysCommand, Unit>.Handle(AddDaysCommand request, CancellationToken cancellationToken)
        {
            var events = await _repository.Load(request.WorkReportId, cancellationToken);
            var reportAddedEvent = events.OfType<ReportAdded>().Single();

            var daysToAdd = request.DaysToAdd.Except(reportAddedEvent.Days);

            if (daysToAdd.Any())
            {
                await _eventBus.Publish(new DaysAdded(request.WorkReportId, daysToAdd));
            }

            return Unit.Value;
        }
    }
}
