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
            AddWorkReport(command.WorkReportId, fixture);

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
            var workReportAddedEvent = AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToAdd, new[] { workReportAddedEvent.Days.PickRandom() })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.Should().ContainSingle();
        }

        [Theory(DisplayName = "Not add the days that already are in the report"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var workReportAddedEvent = AddWorkReport(fixture);
            var effectivelyNewDay = fixture.Create<DateOnly>();
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToAdd, new[] { workReportAddedEvent.Days.PickRandom(), effectivelyNewDay })
                .Create();

            await _context.Sut.Send(addDaysCommand, CancellationToken.None);

            _context.Events.OfType<DaysAdded>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(effectivelyNewDay);
        }

        #region Private

        private ReportAdded AddWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportAddedEvent = fixture.Build<ReportAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _context.Events.Add(workReportAddedEvent);

            return workReportAddedEvent;
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
