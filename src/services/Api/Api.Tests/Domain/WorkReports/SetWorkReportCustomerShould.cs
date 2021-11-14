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
    public class SetWorkReportCustomerShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public SetWorkReportCustomerShould(TestContext context) =>
            _context = context;

        [Theory(DisplayName = "Pass when not specifying a customer"), BoolibaInlineAutoData]
        public async Task Test01(SetWorkReportCustomerCommand command, Fixture fixture)
        {
            _context.AddWorkReport(command.WorkReportId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId)
                .OfType<WorkReportCustomerUnset>()
                .Should().ContainSingle();
        }

        [Theory(DisplayName = "Fail when the given customer does not exist"), BoolibaInlineAutoData(true)]
        public async Task Test02(SetWorkReportCustomerCommand command, Fixture fixture)
        {
            _context.AddWorkReport(command.WorkReportId, fixture);

            Func<Task> t = () => _context.Sut.Send(command, CancellationToken.None);

            await t.Should().ThrowAsync<CustomerNotFoundException>();
            _context.Events(command.WorkReportId)
                .OfType<WorkReportCustomerSet>()
                .Should().BeEmpty();
        }

        [Theory(DisplayName = "Pass when the given customer exists"), BoolibaInlineAutoData(true)]
        public async Task Test03(SetWorkReportCustomerCommand command, Fixture fixture)
        {
            _context.AddCustomer(command.CustomerId!.Value, fixture);
            _context.AddWorkReport(command.WorkReportId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.WorkReportId)
                .OfType<WorkReportCustomerSet>()
                .Should().ContainSingle();
        }
    }
}
