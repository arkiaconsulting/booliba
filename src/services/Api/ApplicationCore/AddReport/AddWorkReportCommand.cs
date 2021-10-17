// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.AddReport
{
    public record AddWorkReportCommand(Guid WorkReportId, string Name, IEnumerable<DateOnly> Days) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddWorkReportCommand>
    {
        private readonly IEventStore _eventStore;

        public AddWorkReportCommandHandler(IEventStore repository) => _eventStore = repository;

        async Task<Unit> IRequestHandler<AddWorkReportCommand, Unit>.Handle(AddWorkReportCommand request, CancellationToken cancellationToken)
        {
            await _eventStore.Save(new ReportAdded(request.WorkReportId, request.Name, request.Days.Distinct()), cancellationToken);

            return Unit.Value;
        }
    }
}