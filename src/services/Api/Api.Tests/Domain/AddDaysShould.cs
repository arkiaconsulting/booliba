// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.AddDaysToReport;
using Booliba.ApplicationCore.AddReport;
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
    public class AddDaysShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public AddDaysShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Produce the right event"), BoolibaInlineAutoData]
        public async Task Test01(AddDaysCommand command, Fixture fixture)
        {
            _context.AddWorkReport(command.WorkReportId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events.OfType<DaysAdded>().Where(e => e.WorkReportId == command.WorkReportId).Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Days = command.DaysToAdd
                });
        }

        [Theory(DisplayName = "Not add the days if they already are in the report"), BoolibaInlineAutoData]
        public async Task Test02(Fixture fixture)
        {
            var (workReportId, initialDays) = _context.AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.DaysToAdd, new[] { initialDays.PickRandom() })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.Should().ContainSingle();
        }

        [Theory(DisplayName = "Not add the days that already are in the report"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var (workReportId, initialDays) = _context.AddWorkReport(fixture);
            var effectivelyNewDay = fixture.Create<DateOnly>();
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.DaysToAdd, new[] { initialDays.PickRandom(), effectivelyNewDay })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysAdded>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(effectivelyNewDay);
        }
    }
}
