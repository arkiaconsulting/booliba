// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;
using Booliba.ApplicationCore.Ports;
using MediatR;

namespace Booliba.ApplicationCore.SendReport
{
    public record SendWorkReportCommand(Guid WorkReportId, string[] EmailAddresses) : IRequest;

    internal class SendWorkReportCommandHandler : IRequestHandler<SendWorkReportCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IEmailNotifier _emailNotifier;

        public SendWorkReportCommandHandler(IEventStore eventStore, IEmailNotifier emailNotifier)
        {
            _eventStore = eventStore;
            _emailNotifier = emailNotifier;
        }

        async Task<Unit> IRequestHandler<SendWorkReportCommand, Unit>.Handle(SendWorkReportCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.Load(request.WorkReportId, cancellationToken);
            if (!events.Any())
            {
                throw new WorkReportNotFoundException(request.WorkReportId);
            }
            var aggregate = WorkReportAggregate.ReHydrate(request.WorkReportId, events);
            aggregate.Send(request.EmailAddresses);

            await _emailNotifier.Send(new EmailMessage(request.WorkReportId, request.EmailAddresses), cancellationToken);

            await _eventStore.Save(aggregate.PendingEvents, cancellationToken);

            return Unit.Value;
        }
    }
}
