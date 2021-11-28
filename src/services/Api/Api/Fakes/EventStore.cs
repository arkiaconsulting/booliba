﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Api.Fakes;
using Booliba.ApplicationCore.Customers;
using Booliba.ApplicationCore.Ports;
using Booliba.ApplicationCore.WorkReports;
using MediatR;

namespace Booliba.Api.Fakes
{
    internal class InMemoryEventStore : IEventStore
    {
        private readonly ICollection<DomainEvent> _events = new List<DomainEvent>();
        private readonly ILogger<InMemoryEventStore> _logger;
        private readonly IMediator _mediator;

        public InMemoryEventStore(
            ILogger<InMemoryEventStore> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        Task<IEnumerable<DomainEvent>> IEventStore.Load(Guid workReportId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Loading events from the event store");

            return Task.FromResult(
                _events
                .Where(e => e.AggregateId == workReportId)
                .ToList()
                .AsEnumerable()
                );
        }

        async Task IEventStore.Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken)
        {
            events.ToList().ForEach(e => _events.Add(e));

            foreach (var @event in events)
            {
                _logger.LogDebug("Saving an event of type {EventType} to the event store", @event.GetType().Name);

                await (@event switch
                {
                    DaysAdded e => _mediator.Publish(new DaysAddedNotification(e), cancellationToken),
                    ReportAdded e => _mediator.Publish(new ReportAddedNotification(e), cancellationToken),
                    DaysRemoved e => _mediator.Publish(new DaysRemovedNotification(e), cancellationToken),
                    WorkReportRemoved e => _mediator.Publish(new WorkReportRemovedNotification(e), cancellationToken),
                    WorkReportSent e => _mediator.Publish(new WorkReportSentNotification(e), cancellationToken),
                    CustomerAdded e => _mediator.Publish(new CustomerAddedNotification(e), cancellationToken),
                    CustomerRemoved e => _mediator.Publish(new CustomerRemovedNotification(e), cancellationToken),
                    _ => throw new InvalidOperationException($"Cannot handle an event of type '{@event.GetType().Name}'")
                });
            }
        }
    }

    internal class NullEmailNotifier : IEmailNotifier
    {
        private readonly ILogger<NullEmailNotifier> _logger;

        public NullEmailNotifier(ILogger<NullEmailNotifier> logger) => _logger = logger;

        Task IEmailNotifier.Send(EmailMessage emailMessage, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Notifying {RecipientCount} recipients of work report {WorkReportId}", emailMessage.EmailAddresses.Length, emailMessage.WorkReportId);

            return Task.CompletedTask;
        }
    }

    #region In memory notifications

    internal record DaysAddedNotification(DaysAdded Event) : INotification;
    internal record ReportAddedNotification(ReportAdded Event) : INotification;
    internal record DaysRemovedNotification(DaysRemoved Event) : INotification;
    internal record WorkReportRemovedNotification(WorkReportRemoved Event) : INotification;
    internal record WorkReportSentNotification(WorkReportSent Event) : INotification;
    internal record CustomerAddedNotification(CustomerAdded Event) : INotification;
    internal record CustomerRemovedNotification(CustomerRemoved Event) : INotification;

    #endregion
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
