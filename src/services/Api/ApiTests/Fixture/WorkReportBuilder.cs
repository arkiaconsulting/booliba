// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Collections.Generic;

namespace Booliba.ApiTests.Fixture
{
    internal class WorkReportBuilder
    {
        private readonly ICollection<DateOnly> _days = new List<DateOnly>();
        private string _name = string.Empty;

        public WorkReportBuilder WithDay(DateOnly day)
        {
            _days.Add(day);

            return this;
        }

        public WorkReportBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public TestWorkReport Build() => new(_name, _days);
    }
}
