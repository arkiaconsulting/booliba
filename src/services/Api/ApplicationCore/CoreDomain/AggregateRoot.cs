// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using System.Reflection;

namespace Booliba.ApplicationCore.CoreDomain
{
    internal abstract class AggregateRoot
    {
        internal IEnumerable<DomainEvent> PendingEvents => _pendingEvents;

        protected readonly Guid _id;
        protected readonly ICollection<DomainEvent> _pendingEvents = new HashSet<DomainEvent>();

        protected AggregateRoot(Guid id) => _id = id;

        public static T ReHydrate<T>(Guid id, IEnumerable<DomainEvent> events)
            where T : AggregateRoot
        {
            if (!events.Any())
            {
                throw new InvalidOperationException("Cannot rehydrate a aggregate without events");
            }

            var constructor = typeof(T)
            .GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                new Type[] { typeof(Guid) }
            );

            if (constructor is null)
            {
                throw new InvalidOperationException($"Cannot find constructor on type {typeof(T).FullName}");
            }

            var aggregate = (T)constructor.Invoke(new object?[] { id });

            foreach (var @event in events)
            {
                aggregate.Apply(@event);
            }

            return aggregate;
        }

        #region Private

        private void Apply(DomainEvent @event)
        {
            try
            {
                var method = this.GetType()
                    .GetRuntimeMethods()
                    .Where(m =>
                        m.Name == nameof(Apply)
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == @event.GetType()
                    ).Single();
                method.Invoke(this, new object[] { @event });
            }
            catch (InvalidOperationException)
            {
                throw new NotImplementedException($"'{nameof(Apply)}({@event.GetType().Name} @event)' is not implemented");
            }
        }

        #endregion
    }
}
