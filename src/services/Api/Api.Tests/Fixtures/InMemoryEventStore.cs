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
        public ICollection<WorkReportEvent> Events => _events;

        private readonly ICollection<WorkReportEvent> _events = new HashSet<WorkReportEvent>();

        Task IEventStore.Save(WorkReportEvent @event, CancellationToken cancellationToken)
        {
            _events.Add(@event);

            return Task.CompletedTask;
        }

        Task<IEnumerable<WorkReportEvent>> IEventStore.Load(Guid workReportId, CancellationToken cancellationToken) =>
            Task.FromResult(_events.Where(e => e.WorkReportId == workReportId).AsEnumerable());
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