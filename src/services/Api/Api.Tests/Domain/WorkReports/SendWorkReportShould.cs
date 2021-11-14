// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore;
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
    public class SendWorkReportShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public SendWorkReportShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Effectively send an existing work report"), BoolibaInlineAutoData]
        public async Task Test01(SendWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Emails(command.WorkReportId)
                .Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    command.WorkReportId,
                    command.EmailAddresses
                });
        }

        [Theory(DisplayName = "Save the right event"), BoolibaInlineAutoData]
        public async Task Test02(SendWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId).OfType<WorkReportSent>()
                .Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    AggregateId = command.WorkReportId,
                    command.EmailAddresses
                });
        }

        [Theory(DisplayName = "Fail when the given work report does not exist"), BoolibaInlineAutoData]
        public async Task Test03(SendWorkReportCommand command)
        {
            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().ThrowAsync<AggregateNotFound>();
        }

        [Theory(DisplayName = "Fail when no recipients are given"), BoolibaInlineAutoData]
        public async Task Test04(Guid workReportId, Fixture fixture)
        {
            _ = _context.AddWorkReport(workReportId, fixture);
            var command = new SendWorkReportCommand(workReportId, Array.Empty<string>());

            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().ThrowAsync<MissingEmailRecipientsException>();
        }

        [Theory(DisplayName = "Send the report multiple times"), BoolibaInlineAutoData]
        public async Task Test05(SendWorkReportCommand command, Fixture fixture)
        {
            _ = _context.AddWorkReport(command.WorkReportId, fixture);
            _context.Send(command.WorkReportId, command.EmailAddresses, fixture);

            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().NotThrowAsync<NotImplementedException>();
        }

        [Theory(DisplayName = "Cannot send to recipients when the report has been removed"), BoolibaInlineAutoData]
        public async Task Test06(SendWorkReportCommand command, Fixture fixture)
        {
            _context.AddWorkReport(command.WorkReportId, fixture);
            _context.RemoveWorkReport(command.WorkReportId, fixture);

            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().ThrowAsync<WorkReportRemovedException>();
        }
    }
}
