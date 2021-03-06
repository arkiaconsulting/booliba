// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.QuerySide;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Query
{
    [Trait("Category", "Unit")]
    public class WorkReportProjectionServiceShould
    {
        private readonly TestContext _context;

        public WorkReportProjectionServiceShould() => _context = new TestContext();

        [Theory(DisplayName = "Project a new work report"), BoolibaInlineAutoData]
        public async Task Test01(Guid workReportId, string name, DateOnly[] days, Guid customerId)
        {
            await _context.ProjectionService.CreateWorkReport(workReportId, name, days, customerId);

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, name, days, Array.Empty<string>(), customerId)
            );
        }

        [Theory(DisplayName = "Project when days are added to an existing report"), BoolibaInlineAutoData]
        public async Task Test02(Guid workReportId, string workReportName, DateOnly[] days, DateOnly[] daysToAdd)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);

            await _context.ProjectionService.AddDays(workReportId, daysToAdd);

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, days.Concat(daysToAdd).ToArray(), Array.Empty<string>())
            );
        }

        [Theory(DisplayName = "Project when days are removed from an existing report"), BoolibaInlineAutoData]
        public async Task Test03(Guid workReportId, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);
            var daysToRemove = days.PickRandom(days.Length - 1).ToArray();

            await _context.ProjectionService.RemoveDays(workReportId, daysToRemove);

            var expectedDays = days.Except(daysToRemove).ToArray();
            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, expectedDays, Array.Empty<string>())
            );
        }

        [Theory(DisplayName = "Project when an existing report is removed"), BoolibaInlineAutoData]
        public async Task Test04(Guid workReportId, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);

            await _context.ProjectionService.RemoveWorkReport(workReportId);

            _context.WorkReportEntities.Should().NotContainEquivalentOf(new
            {
                Id = workReportId
            });
        }

        [Theory(DisplayName = "Project when an existing report is sent to email recipients"), BoolibaInlineAutoData]
        public async Task Test06(Guid workReportId, string workReportName, DateOnly[] days, string[] recipientEmails)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);

            await _context.ProjectionService.AddRecipients(workReportId, recipientEmails);

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, days, recipientEmails)
            );
        }

        [Theory(DisplayName = "Not store duplicates when sending an existing report to email recipients"), BoolibaInlineAutoData]
        public async Task Test07(Guid workReportId, string workReportName, DateOnly[] days, string[] recipientEmails)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days, recipientEmails);

            await _context.ProjectionService.AddRecipients(workReportId, recipientEmails.PickRandom(1).ToArray());

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, days, recipientEmails)
            );
        }

        [Theory(DisplayName = "Project when the customer is set on the work report"), BoolibaInlineAutoData]
        public async Task Test08(Guid workReportId, Guid customerId, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);

            await _context.ProjectionService.SetCustomer(workReportId, customerId);

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, days.ToArray(), Array.Empty<string>(), customerId)
            );
        }

        [Theory(DisplayName = "Project when the customer is unset on the work report"), BoolibaInlineAutoData]
        public async Task Test09(Guid workReportId, string workReportName, DateOnly[] days)
        {
            _context.AddProjectedWorkReport(workReportId, workReportName, days);

            await _context.ProjectionService.SetCustomer(workReportId, default);

            _context.WorkReportEntities.Should().ContainEquivalentOf(
                new WorkReportEntity(workReportId, workReportName, days.ToArray(), Array.Empty<string>(), default)
            );
        }
    }
}
