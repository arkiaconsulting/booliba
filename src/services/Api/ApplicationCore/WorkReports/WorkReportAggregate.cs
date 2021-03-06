// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.CoreDomain;

namespace Booliba.ApplicationCore.WorkReports
{
    internal class WorkReportAggregate : AggregateRoot
    {
        private readonly HashSet<DateOnly> _days = new();
        private readonly HashSet<string> _sentToEmails = new();
        private string _name = string.Empty;
        private bool _removed;
        private Guid? _customerId;

        private WorkReportAggregate(Guid id) : base(id) { }

        public static WorkReportAggregate Create(Guid id, string name, IEnumerable<DateOnly> days, Guid? customerId)
        {
            var aggregate = new WorkReportAggregate(id);
            var @event = new ReportAdded(id, name, days.Distinct(), customerId);

            aggregate.Apply(@event);

            aggregate._pendingEvents.Add(@event);

            return aggregate;
        }

        internal void Remove()
        {
            var @event = new WorkReportRemoved(_id);
            Apply(@event);

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
            Apply(@event);

            _pendingEvents.Add(@event);
        }

        internal void RemoveDays(IEnumerable<DateOnly> daysToRemove)
        {
            ThrowIfRemoved();

            var daysToRemoveEffectively = daysToRemove.Intersect(_days).ToList();
            var @event = new DaysRemoved(_id, daysToRemoveEffectively);

            Apply(@event);

            if (daysToRemoveEffectively.Any())
            {
                _pendingEvents.Add(@event);
            }
        }

        internal void SetCustomer(Guid customerId)
        {
            var @event = new WorkReportCustomerSet(_id, customerId);

            Apply(@event);

            _pendingEvents.Add(@event);
        }


        internal void UnsetCustomer()
        {
            var @event = new WorkReportCustomerUnset(_id);

            Apply(@event);

            _pendingEvents.Add(@event);
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
            Apply(@event);

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

        private void Apply(ReportAdded @event)
        {
            _name = @event.WorkReportName;
            foreach (var day in @event.Days)
            {
                _days.Add(day);
            }
            _customerId = @event.CustomerId;
        }

        private void Apply(DaysAdded @event)
        {
            foreach (var day in @event.Days)
            {
                _days.Add(day);
            }
        }

        private void Apply(DaysRemoved @event)
        {
            foreach (var day in @event.Days)
            {
                _days.Remove(day);
            }
        }

        private void Apply(WorkReportRemoved _) =>
            _removed = true;

        private void Apply(WorkReportSent @event) =>
            @event.EmailAddresses.ToList().ForEach(email => _sentToEmails.Add(email));

        private void Apply(WorkReportCustomerUnset _) =>
            _customerId = default;

        private void Apply(WorkReportCustomerSet @event) =>
            _customerId = @event.CustomerId;

        #endregion
    }
}
