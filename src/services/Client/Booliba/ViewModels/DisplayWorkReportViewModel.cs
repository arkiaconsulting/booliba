// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Collections.Generic;
using System.Linq;

namespace Booliba.ViewModels
{
    internal class DisplayWorkReportViewModel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public IEnumerable<IGrouping<int, DateOnly>> DaysPerYear { get; private set; }
        public int TotalDayCount { get; private set; }

        public DisplayWorkReportViewModel(
            Guid id,
            string name,
            IEnumerable<DateOnly> days)
        {
            Id = id;
            Name = name;
            DaysPerYear = days.GroupBy(d => d.Year);
            TotalDayCount = days.Count();
        }
    }
}
