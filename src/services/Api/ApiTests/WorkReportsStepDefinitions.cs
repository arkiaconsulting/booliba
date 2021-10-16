using Booliba.ApiTests.Fixture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Booliba.ApiTests
{
    [Binding]
    public sealed class WorkReportsStepDefinitions
    {
        private readonly TestContext _context;
        private Guid _workReportId;

        public WorkReportsStepDefinitions(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have prepared my report for the current month")]
        public void GivenIHavePreparedMyReportForTheCurrentMonth()
        {
            _context.CurrentWorkReport = new WorkReportBuilder()
                .WithDay(Some.DayInCurrentMonth)
                .WithDay(Some.DayInCurrentMonth)
                .Build();
        }

        [When(@"I add my work report")]
        public async Task WhenIAddMyWorkReport()
        {
            _workReportId = Guid.NewGuid();
            var workReportDto = new WorkReportDto(_workReportId, _context.CurrentWorkReport);

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "reports")
            {
                Content = JsonContent.Create(workReportDto)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see my work report into the work report list")]
        public async Task ThenICanSeeThatMyReportHasBeenSavedAsync()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "reports");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }
    }

    internal record WorkReportDto(Guid Id, IEnumerable<DateTimeOffset> Days);
}
