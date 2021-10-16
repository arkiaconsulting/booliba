// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace Booliba.Tests.Fixtures
{
    internal class InMemoryEventBus : IEventBus
    {
        public IEnumerable<DomainEvent> Events => _events;

        private ICollection<DomainEvent> _events = new HashSet<DomainEvent>();

        Task IEventBus.Publish(DomainEvent @event)
        {
            _events.Add(@event);

            return Task.CompletedTask;
        }
    }
}
