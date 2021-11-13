// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Customers;

namespace Booliba.ApplicationCore.CoreDomain
{
    internal class CustomerAggregate : AggregateRoot
    {
        private string _name = string.Empty;

        private CustomerAggregate(Guid id) : base(id) { }

        internal static CustomerAggregate Create(Guid id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new AddCustomerException("A customer should have a name");
            }

            var aggregate = new CustomerAggregate(id);
            var @event = new CustomerAdded(id, name);

            aggregate.Apply(@event);
            aggregate._pendingEvents.Add(@event);

            return aggregate;
        }

        internal void Remove() => _pendingEvents.Add(new CustomerRemoved(_id));

        #region Event handlers

        private void Apply(CustomerAdded @event) => _name = @event.Name;

        private void Apply(CustomerRemoved @event) { }

        #endregion
    }
}
