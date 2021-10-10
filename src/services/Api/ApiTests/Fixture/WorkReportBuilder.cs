using System;
using System.Collections.Generic;

namespace Booliba.ApiTests.Fixture
{
    internal class WorkReportBuilder
    {
        private ICollection<DateTimeOffset> _days = new List<DateTimeOffset>();

        public WorkReportBuilder WithDay(DateTimeOffset day)
        {
            _days.Add(day);

            return this;
        }

        public ICollection<DateTimeOffset> Build() => _days;
    }
}
