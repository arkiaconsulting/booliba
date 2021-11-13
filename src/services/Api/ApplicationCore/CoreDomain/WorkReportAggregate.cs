// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using Booliba.ApplicationCore.RemoveDaysFromReport;
using Booliba.ApplicationCore.RemoveWorkReport;
using Booliba.ApplicationCore.SendReport;
using System.Reflection;

namespace Booliba.ApplicationCore.CoreDomain
{
    internal class WorkReportAggregate
    {
        internal IEnumerable<DomainEvent> PendingEvents => _pendingEvents;

        private readonly Guid _id;
        private readonly HashSet<DateOnly> _days = new();
        private readonly HashSet<string> _sentToEmails = new();
        private readonly ICollection<DomainEvent> _pendingEvents = new HashSet<DomainEvent>();
        private string _name = string.Empty;
        private bool _removed;

        private WorkReportAggregate(Guid id) => _id = id;

        public static WorkReportAggregate Create(Guid id, string name, IEnumerable<DateOnly> days)
        {
            var aggregate = new WorkReportAggregate(id);
            var @event = new ReportAdded(id, name, days.Distinct());

            aggregate.On(@event);

            aggregate._pendingEvents.Add(@event);

            return aggregate;
        }

        internal void Remove()
        {
            var @event = new WorkReportRemoved(_id);
            On(@event);

            _pendingEvents.Add(@event);
        }

        internal void Send(string[] emailAddresses)
        {
            ThrowIfRemoved();

            if (!emailAddresses.Any())
            {
                throw new MissingEmailRecipientsException(_id);
            }

            var @event = new WorkReportSent(_id, emailAddresses);
            On(@event);

            _pendingEvents.Add(@event);
        }

        internal void RemoveDays(IEnumerable<DateOnly> daysToRemove)
        {
            ThrowIfRemoved();

            var daysToRemoveEffectively = daysToRemove.Intersect(_days).ToList();
            var @event = new DaysRemoved(_id, daysToRemoveEffectively);

            On(@event);

            if (daysToRemoveEffectively.Any())
            {
                _pendingEvents.Add(@event);
            }
        }

        internal void AddDays(IEnumerable<DateOnly> daysToAdd)
        {
            ThrowIfRemoved();

            var effectiveDaysToAdd = daysToAdd.Except(_days.Intersect(daysToAdd)).ToList();
            if (!effectiveDaysToAdd.Any())
            {
                return;
            }

            var @event = new DaysAdded(_id, effectiveDaysToAdd);
            On(@event);

            _pendingEvents.Add(@event);
        }

        private void ThrowIfRemoved()
        {
            if (_removed)
            {
                throw new WorkReportRemovedException(_id);
            }
        }

        #region Event handlers

        private void On(ReportAdded @event)
        {
            _name = @event.WorkReportName;
            foreach (var day in @event.Days)
            {
                _days.Add(day);
            }
        }

        private void On(DaysAdded @event)
        {
            foreach (var day in @event.Days)
            {
                _days.Add(day);
            }
        }

        private void On(DaysRemoved @event)
        {
            foreach (var day in @event.Days)
            {
                _days.Remove(day);
            }
        }

        private void On(WorkReportRemoved _) =>
            _removed = true;

        private void On(WorkReportSent @event) =>
            @event.EmailAddresses.ToList().ForEach(email => _sentToEmails.Add(email));

        #endregion

        #region Re-hydrate

        internal static WorkReportAggregate ReHydrate(Guid id, IEnumerable<DomainEvent> events)
        {
            var aggregate = new WorkReportAggregate(id);

            foreach (var @event in events)
            {
                aggregate.Apply(@event);
            }

            return aggregate;
        }

        private void Apply(DomainEvent @event)
        {
            try
            {
                var method = typeof(WorkReportAggregate)
                    .GetRuntimeMethods()
                    .Where(m =>
                        m.Name == nameof(On)
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == @event.GetType()
                    ).Single();
                method.Invoke(this, new object[] { @event });
            }
            catch (InvalidOperationException)
            {
                throw new NotImplementedException($"'On({@event.GetType().Name} @event)' is not implemented");
            }
        }

        #endregion
    }
}
