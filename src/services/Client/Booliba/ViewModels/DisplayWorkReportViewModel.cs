// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booliba.ViewModels
{
    public class DisplayWorkReportViewModel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Customer? Customer { get; private set; }
        public IEnumerable<IGrouping<int, DateOnly>>? DaysPerYear { get; private set; } = default;
        public int TotalDayCount { get; private set; }
        public IEnumerable<string> Recipients => _recipients;

        private readonly HashSet<DateOnly> _days;
        private readonly HashSet<string> _recipients;
        private readonly BoolibaService _boolibaService;

        public DisplayWorkReportViewModel(
            BoolibaService boolibaService,
            Guid id,
            string name,
            IEnumerable<DateOnly> days,
            IEnumerable<string> recipients,
            Customer? customer)
        {
            _boolibaService = boolibaService;
            Id = id;
            Name = name;
            Customer = customer;
            _days = days.ToHashSet();
            _recipients = new HashSet<string>(recipients);
            TotalDayCount = days.Count();
            UpdateExposedDayGroup();
        }

        public async Task Switch(DateOnly day)
        {
            if (_days.Contains(day))
            {
                await _boolibaService.RemoveDay(Id, day);
                _days.Remove(day);
            }
            else
            {
                await _boolibaService.AddDay(Id, day);
                _days.Add(day);
            }

            UpdateExposedDayGroup();
        }

        internal async Task Fill(int year, int month)
        {
            var monthDaysWithoutWeekEndDays = WorkReportHelpers.GetMonthDays(year, month)
                .Where(d => !new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(d.DayOfWeek));

            await _boolibaService.AddDays(Id, monthDaysWithoutWeekEndDays.ToArray());

            foreach (var dayToAdd in monthDaysWithoutWeekEndDays)
            {
                _days.Add(dayToAdd);
            }

            UpdateExposedDayGroup();
        }

        internal async Task Empty(int year, int month)
        {
            var daysToRemove = WorkReportHelpers.GetMonthDays(year, month);

            await _boolibaService.RemoveDays(Id, daysToRemove.ToArray());

            foreach (var day in daysToRemove)
            {
                _days.Remove(day);
            }

            UpdateExposedDayGroup();
        }

        internal async Task AddMonth(int year, int month)
        {
            var monthDaysWithoutWeekEndDays = WorkReportHelpers.GetMonthDays(year, month)
                .Where(d => !new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(d.DayOfWeek));

            await _boolibaService.AddDays(Id, monthDaysWithoutWeekEndDays.ToArray());

            foreach (var day in monthDaysWithoutWeekEndDays)
            {
                _days.Add(day);
            }

            UpdateExposedDayGroup();
        }

        internal async Task SetCustomer(Customer customer)
        {
            await _boolibaService.SetWorkReportCustomer(Id, customer.Id);

            Customer = customer;
        }

        public async Task Remove() =>
            await _boolibaService.Remove(Id);

        public void RemoveRecipient(string recipient) =>
            _recipients.Remove(recipient);

        public void AddRecipient(string recipient) =>
            _recipients.Add(recipient);

        public async Task Send() =>
            await _boolibaService.Send(Id, _recipients);

        private void UpdateExposedDayGroup()
        {
            DaysPerYear = new List<IGrouping<int, DateOnly>>(_days.GroupBy(d => d.Year));
            TotalDayCount = _days.Count;
        }
    }
}
