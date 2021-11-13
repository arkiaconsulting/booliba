// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booliba.Tests.Fixtures
{
    internal class InMemoryEventStore : IEventStore
    {
        public ICollection<DomainEvent> Events => _events;

        private readonly ICollection<DomainEvent> _events = new HashSet<DomainEvent>();

        Task IEventStore.Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken)
        {
            events.ToList().ForEach(e => _events.Add(e));

            return Task.CompletedTask;
        }

        Task<IEnumerable<DomainEvent>> IEventStore.Load(Guid workReportId, CancellationToken cancellationToken) =>
            Task.FromResult(_events.Where(e => e.AggregateId == workReportId).AsEnumerable());
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services) =>
            services.AddSingleton<IEventStore, InMemoryEventStore>();
    }
}