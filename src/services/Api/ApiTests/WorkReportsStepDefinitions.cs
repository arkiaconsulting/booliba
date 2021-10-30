// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApiTests.Fixture;
using FluentAssertions;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Booliba.ApiTests
{
    [Binding]
    public sealed class WorkReportsStepDefinitions
    {
        private readonly TestContext _context;
        private Guid _workReportId;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public WorkReportsStepDefinitions(TestContext context)
        {
            _context = context;
            _jsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        }

        [Given(@"I have prepared my report for the current month")]
        public void GivenIHavePreparedMyReportForTheCurrentMonth()
        {
            _workReportId = Guid.NewGuid();
            _context.CurrentWorkReport = new WorkReportBuilder()
                .WithDay(Some.DayInCurrentMonth)
                .WithDay(Some.DayInCurrentMonth)
                .WithName($"name-{Guid.NewGuid()}")
                .Build();
        }

        [When(@"I add my work report")]
        public async Task WhenIAddMyWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}")
            {
                Content = JsonContent.Create(_context.CurrentWorkReport, options: _jsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see my work report into the work report list")]
        public async Task ThenICanSeeThatMyReportHasBeenSavedAsync()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "workreports");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var reports = await response.Content.ReadFromJsonAsync<WorkReportDto[]>();
            reports.Should().ContainEquivalentOf(new
            {
                Id = _workReportId
            });
        }

        [Given(@"I have added a work report")]
        public async Task GivenIHaveAddedAWorkReport()
        {
            _workReportId = Guid.NewGuid();
            _context.CurrentWorkReport = new WorkReportBuilder()
                .WithDay(Some.DayInCurrentMonth)
                .WithDay(Some.DayInCurrentMonth)
                .WithName($"name-{Guid.NewGuid()}")
                .Build();

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}")
            {
                Content = JsonContent.Create(_context.CurrentWorkReport, options: _jsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [When(@"I add '([^']*)' to the work report")]
        public async Task WhenIAddToTheWorkReport(string day)
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { day }
                }, options: _jsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see that the day '([^']*)' has been added to the days list")]
        public async Task ThenICanSeeThatTheDayHasBeenAddedToTheDaysList(string day)
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports/{_workReportId}");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var report = await response.Content.ReadFromJsonAsync<WorkReportDto>();
            report!.Days.Should().Contain(DateOnly.ParseExact(day, "yyyy/MM/dd"));
        }

        [When(@"I remove one of the days from the work report")]
        public async Task WhenIRemoveOneOfTheDaysFromTheWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{_workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { _context.CurrentWorkReport!.Days.PickRandom() }
                }, options: _jsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see that my work report contains one day less")]
        public async Task ThenICanSeeThatMyWorkReportContainsOneDayLess()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports/{_workReportId}");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var report = await response.Content.ReadFromJsonAsync<WorkReportDto>();
            report!.Days.Should().HaveCount(_context.CurrentWorkReport!.Days.Count() - 1);
        }

        [When(@"I remove my work report")]
        public async Task WhenIRemoveMyWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{_workReportId}");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see that my work report is no longer in the work report list")]
        public async Task ThenICanSeeThatMyWorkReportIsNoLongerInTheWorkReportList()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var reports = await response.Content.ReadFromJsonAsync<WorkReportDto[]>();
            reports.Should().NotContainEquivalentOf(new
            {
                Id = _workReportId
            });
        }
    }
}
