// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Api.Fakes;
using Booliba.ApplicationCore.Ports;
using Booliba.ApplicationCore.SendReport;

namespace Booliba.Api.Fakes
{
    internal class InMemoryEventStore : IEventStore
    {
        private readonly ICollection<WorkReportEvent> _events = new List<WorkReportEvent>();
        private readonly ILogger<InMemoryEventStore> _logger;

        public InMemoryEventStore(ILogger<InMemoryEventStore> logger) => _logger = logger;

        Task<IEnumerable<WorkReportEvent>> IEventStore.Load(Guid workReportId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading events from the event store");

            return Task.FromResult(
                _events
                .Where(e => e.WorkReportId == workReportId)
                .ToList()
                .AsEnumerable()
                );
        }

        Task IEventStore.Save(IEnumerable<WorkReportEvent> events, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saving {EventCount} events to the event store", events.Count());

            events.ToList().ForEach(e => _events.Add(e));

            return Task.CompletedTask;
        }
    }

    internal class NullEmailNotifier : IEmailNotifier
    {
        private readonly ILogger<NullEmailNotifier> _logger;

        public NullEmailNotifier(ILogger<NullEmailNotifier> logger) => _logger = logger;

        Task IEmailNotifier.Send(EmailMessage emailMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notifying {RecipientCount} recipients of work report {WorkReportId}", emailMessage.EmailAddresses.Length, emailMessage.WorkReportId);

            return Task.CompletedTask;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class EventStoreExtensions
    {
        public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services) =>
            services.AddSingleton<IEventStore, InMemoryEventStore>();
    }

    internal static class EmailNotifierExtensions
    {
        public static IServiceCollection AddNullEmailNotifier(this IServiceCollection services) =>
            services.AddTransient<IEmailNotifier, NullEmailNotifier>();
    }
}
