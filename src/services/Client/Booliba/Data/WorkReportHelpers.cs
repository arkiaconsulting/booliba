// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booliba.Data
{
    internal static class WorkReportHelpers
    {
        public static IEnumerable<DateOnly> GetMonthDays(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(dayNumber => new DateOnly(year, month, dayNumber));
        }
    }
}
