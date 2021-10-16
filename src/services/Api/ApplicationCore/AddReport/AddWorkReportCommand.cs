// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.AddReport
{
    public record AddWorkReportCommand(Guid Id, string Name, IEnumerable<DateOnly> Days) : IRequest;

    internal class AddWorkReportCommandHandler : IRequestHandler<AddWorkReportCommand>
    {
        private readonly IEventBus _eventBus;

        public AddWorkReportCommandHandler(IEventBus eventBus) => _eventBus = eventBus;

        async Task<Unit> IRequestHandler<AddWorkReportCommand, Unit>.Handle(AddWorkReportCommand request, CancellationToken cancellationToken)
        {
            await _eventBus.Publish(new ReportAdded(request.Id, request.Name, request.Days.Distinct()));

            return Unit.Value;
        }
    }
}