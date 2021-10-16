// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.RemoveDaysFromReport;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain
{
    [Trait("Category", "Unit")]
    public class RemoveDaysShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public RemoveDaysShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Effectively remove a day from the report"), BoolibaInlineAutoData]
        public async Task Test01(Fixture fixture)
        {
            var reportAddedEvent = AddWorkReport(fixture);
            var dayToRemove = reportAddedEvent.Days.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, reportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { dayToRemove })
                .Create();

            await _context.Sut.Send(removeDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysRemoved>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(dayToRemove);
        }

        [Theory(DisplayName = "Not remove a day that was not in the report"), BoolibaInlineAutoData]
        public async Task Test02(Fixture fixture)
        {
            var workReportAddedEvent = AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { fixture.Create<DateOnly>() })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysRemoved>().Where(e => e.WorkReportId == workReportAddedEvent.WorkReportId).Should().BeEmpty();
        }

        [Theory(DisplayName = "Remove a day that was previously added"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var reportAddedEvent = AddWorkReport(fixture);
            var daysAddedEvent = AddDays(reportAddedEvent.WorkReportId, fixture);

            var dayToRemove = daysAddedEvent.Days.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, reportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { dayToRemove })
                .Create();

            await _context.Sut.Send(removeDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysRemoved>().Where(e => e.WorkReportId == reportAddedEvent.WorkReportId).Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(dayToRemove);
        }

        #region Private

        private DaysAdded AddDays(Guid workReportId, Fixture fixture)
        {
            var daysAdded = fixture.Build<DaysAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _context.Events.Add(daysAdded);

            return daysAdded;
        }

        private ReportAdded AddWorkReport(Fixture fixture)
        {
            var workReportAddedEvent = fixture.Create<ReportAdded>();

            _context.Events.Add(workReportAddedEvent);

            return workReportAddedEvent;
        }

        #endregion
    }
}
