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
    public sealed class WorkReportsStepDefinitions : IDisposable
    {
        private HttpResponseMessage AddWorkReportResponse => _addWorkReportResponse ?? throw new InvalidOperationException("A work report has not been added");

        private readonly TestContext _context;
        private HttpResponseMessage? _addWorkReportResponse;

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
            var workReportDto = new WorkReportDto(Guid.NewGuid(), _context.CurrentWorkReport);

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "reports")
            {
                Content = JsonContent.Create(workReportDto)
            };

            _addWorkReportResponse = await httpClient.SendAsync(httpRequest);
        }

        [Then(@"I can see that my work report has been saved")]
        public void ThenICanSeeThatMyReportHasBeenSaved()
        {
            AddWorkReportResponse.EnsureSuccessStatusCode();
        }

        public void Dispose() => ((IDisposable?)_addWorkReportResponse)?.Dispose();
    }

    internal record WorkReportDto(Guid Id, IEnumerable<DateTimeOffset> Days);
}
