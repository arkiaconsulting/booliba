// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
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
    public class RemoveWorkReportShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public RemoveWorkReportShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Effectively remove an existing work report"), BoolibaInlineAutoData]
        public async Task Test01(RemoveWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<WorkReportRemoved>()
                .Should().ContainSingle();
        }

        [Theory(DisplayName = "Pass when removing a work report that does not exist"), BoolibaInlineAutoData]
        public async Task Test02(RemoveWorkReportCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<WorkReportRemoved>()
                .Should().BeEmpty();
        }

        [Theory(DisplayName = "Pass when removing a work report that has already been removed"), BoolibaInlineAutoData]
        public async Task Test03(RemoveWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);
            _context.RemoveWorkReport(command.WorkReportId, fixture);

            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().NotThrowAsync<NotImplementedException>();
        }
    }
}
