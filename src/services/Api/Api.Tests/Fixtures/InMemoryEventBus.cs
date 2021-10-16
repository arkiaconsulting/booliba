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
    internal class InMemoryEventBus : IEventBus, IRepository
    {
        public ICollection<DomainEvent> Events => _events;

        private readonly ICollection<DomainEvent> _events = new HashSet<DomainEvent>();

        Task IEventBus.Publish(DomainEvent @event, CancellationToken cancellationToken)
        {
            _events.Add(@event);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<DomainEvent>> Load(Guid workReportId, CancellationToken cancellationToken) => Task.FromResult(_events.AsEnumerable());
    }

}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
        {
            var o = new InMemoryEventBus();
            return services
                .AddSingleton<IEventBus>(_ => o)
                .AddSingleton<IRepository>(_ => o);
        }
    }
}