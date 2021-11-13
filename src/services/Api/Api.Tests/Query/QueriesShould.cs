// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.QuerySide;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Query
{
    [Trait("Category", "Unit")]
    public class QueriesShould
    {
        private readonly TestContext _context;

        public QueriesShould() => _context = new TestContext();

        [Theory(DisplayName = "Return an empty list when no work reports are found")]
        [BoolibaInlineAutoData]
        public async Task Test01(GetWorkReportsQuery query) => await _context.Sut.Send(query);

        [Theory(DisplayName = "Return all the work reports")]
        [BoolibaInlineAutoData]
        public async Task Test02(GetWorkReportsQuery query, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(Guid.NewGuid(), workReportName, days);

            var response = await _context.Sut.Send(query);

            response.Results.Should().NotBeEmpty();
            response.Results.Should().ContainSingle();
        }

        [Theory(DisplayName = "Return nothing when the given work report is not stored")]
        [BoolibaInlineAutoData]
        public async Task Test03(GetWorkReportQuery query)
        {
            var response = await _context.Sut.Send(query);

            response.Result.Should().BeNull();
        }

        [Theory(DisplayName = "Return the given work report when it is stored")]
        [BoolibaInlineAutoData]
        public async Task Test04(GetWorkReportQuery query, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(query.WorkReportId, workReportName, days);

            var response = await _context.Sut.Send(query);

            response.Result.Should().NotBeNull();
        }

        [Theory(DisplayName = "Return all the customers")]
        [BoolibaInlineAutoData]
        public async Task Test05(GetCustomersQuery query, string customerName)
        {
            _context.AddProjectedCustomer(Guid.NewGuid(), customerName);

            var response = await _context.Sut.Send(query);

            response.Results.Should().NotBeEmpty();
            response.Results.Should().ContainSingle();
        }
    }
}
