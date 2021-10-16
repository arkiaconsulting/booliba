// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
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
            var (workReportId, initialDays) = _context.AddWorkReport(fixture);
            var dayToRemove = initialDays.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, workReportId)
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
            var (workReportId, _) = _context.AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.DaysToRemove, new[] { fixture.Create<DateOnly>() })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysRemoved>().Where(e => e.WorkReportId == workReportId).Should().BeEmpty();
        }

        [Theory(DisplayName = "Remove a day that was previously added"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var (workReportId, initialDays) = _context.AddWorkReport(fixture);
            var daysAddedEvent = _context.AddDays(workReportId, fixture);

            var dayToRemove = initialDays.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.DaysToRemove, new[] { dayToRemove })
                .Create();

            await _context.Sut.Send(removeDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysRemoved>().Where(e => e.WorkReportId == workReportId).Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(dayToRemove);
        }
    }
}
