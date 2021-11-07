// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booliba.ViewModels
{
    internal class DisplayWorkReportViewModel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public IEnumerable<IGrouping<int, DateOnly>>? DaysPerYear { get; private set; } = default;
        public int TotalDayCount { get; private set; }

        private readonly ICollection<DateOnly> _days;
        private readonly BoolibaService _boolibaService;

        public DisplayWorkReportViewModel(
            BoolibaService boolibaService,
            Guid id,
            string name,
            IEnumerable<DateOnly> days)
        {
            _boolibaService = boolibaService;
            Id = id;
            Name = name;
            _days = days.ToList();
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

        private void UpdateExposedDayGroup() =>
            DaysPerYear = new List<IGrouping<int, DateOnly>>(_days.GroupBy(d => d.Year));
    }
}
