// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;

namespace Booliba.ApplicationCore.Customers
{
    internal class CustomerService
    {
        private readonly IEventStore _eventStore;

        public CustomerService(IEventStore eventStore) => _eventStore = eventStore;

        internal async Task<bool> Exists(Guid customerId, CancellationToken cancellationToken = default)
        {
            var events = await _eventStore.Load(customerId, cancellationToken);

            return events.Any();
        }
    }
}
