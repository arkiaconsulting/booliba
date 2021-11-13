// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Customers;
using Booliba.ApplicationCore.Ports;

namespace Booliba.ApplicationCore.CoreDomain
{
    internal class CustomerAggregate
    {
        internal IEnumerable<DomainEvent> PendingEvents => _pendingEvents;

        private readonly ICollection<DomainEvent> _pendingEvents = new HashSet<DomainEvent>();
        private readonly Guid _id;
        private string _name = string.Empty;

        private CustomerAggregate(Guid id) => _id = id;

        internal static CustomerAggregate Create(Guid id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new AddCustomerException();
            }

            var aggregate = new CustomerAggregate(id);
            var @event = new CustomerAdded(id, name);

            aggregate.On(@event);
            aggregate._pendingEvents.Add(@event);

            return aggregate;
        }

        #region Event handlers

        private void On(CustomerAdded @event) => _name = @event.Name;

        #endregion
    }
}
