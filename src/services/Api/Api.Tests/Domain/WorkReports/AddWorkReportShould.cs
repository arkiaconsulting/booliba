// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.WorkReports;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain.WorkReports
{
    [Trait("Category", "Unit")]
    public class AddWorkReportShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public AddWorkReportShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Pass"), BoolibaInlineAutoData]
        public async Task Test01(AddWorkReportCommand command) =>
            await _context.Sut.Send(command, CancellationToken.None);

        [Theory(DisplayName = "Produce an event"), BoolibaInlineAutoData]
        public async Task Test02(AddWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<ReportAdded>().Should().ContainSingle();
        }

        [Theory(DisplayName = "Handle setting the work days"), BoolibaInlineAutoData]
        public async Task Test03(AddWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<ReportAdded>().Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    command.Days
                });
        }

        [Theory(DisplayName = "Uniquely identify a work report"), BoolibaInlineAutoData]
        public async Task Test04(AddWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<ReportAdded>().Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    AggregateId = command.WorkReportId
                });
        }

        [Theory(DisplayName = "Remove day duplicates"), BoolibaInlineAutoData]
        public async Task Test05(Guid workReportId, string workReportName)
        {
            var command = new AddWorkReportCommand(workReportId, workReportName, new[]
            {
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now)
            });

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(workReportId).OfType<ReportAdded>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle();
        }

        [Theory(DisplayName = "Handle setting the work report name"), BoolibaInlineAutoData]
        public async Task Test06(AddWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    WorkReportName = command.Name
                });
        }
    }
}
