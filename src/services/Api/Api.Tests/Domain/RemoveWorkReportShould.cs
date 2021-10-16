// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.RemoveWorkReport;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain
{
    [Trait("Category", "Unit")]
    public class RemoveWorkReportShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public RemoveWorkReportShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Effectively remove an existing work report"), BoolibaInlineAutoData]
        public async Task Test01(RemoveWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events.OfType<WorkReportRemoved>().Where(e => e.WorkReportId == command.WorkReportId)
                .Should().ContainSingle();
        }

        [Theory(DisplayName = "Not remove a non existing report"), BoolibaInlineAutoData]
        public async Task Test02(RemoveWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events.OfType<WorkReportRemoved>().Where(e => e.WorkReportId == command.WorkReportId)
                .Should().BeEmpty();
        }
    }
}
