// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApiTests.Fixture;
using FluentAssertions;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Sdk;

namespace Booliba.ApiTests
{
    [Binding]
    public sealed class WorkReportsStepDefinitions
    {
        private readonly TestContext _context;
        private Guid _workReportId;

        public WorkReportsStepDefinitions(TestContext context) => _context = context;

        [Given(@"I have prepared my report for the current month")]
        public void GivenIHavePreparedMyReportForTheCurrentMonth()
        {
            _workReportId = Guid.NewGuid();
            _context.CurrentWorkReport = new WorkReportBuilder()
                .WithDay(Some.DayInCurrentMonth)
                .WithDay(Some.DayInCurrentMonth)
                .WithName($"name-{Guid.NewGuid()}")
                .WithCustomer(_context.CustomerId)
                .Build();
        }

        [When(@"I add my work report")]
        public async Task WhenIAddMyWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}")
            {
                Content = JsonContent.Create(_context.CurrentWorkReport, options: _context.JsonSerializerOptions)
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

            var reports = await response.Content.ReadFromJsonAsync<WorkReportDto[]>(_context.JsonSerializerOptions);
            reports.Should().ContainEquivalentOf(new
            {
                Id = _workReportId,
                CustomerId = _context.CustomerId ?? default(Guid?)
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
                Content = JsonContent.Create(_context.CurrentWorkReport, options: _context.JsonSerializerOptions)
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
                }, options: _context.JsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [When(@"I set the customer of my work report")]
        public async Task WhenISetTheCustomerOfMyWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}/customer")
            {
                Content = JsonContent.Create(new
                {
                    _context.CustomerId,
                }, options: _context.JsonSerializerOptions)
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

            var report = await response.Content.ReadFromJsonAsync<WorkReportDto>(_context.JsonSerializerOptions);
            report!.Days.Should().Contain(DateOnly.ParseExact(day, "yyyy/MM/dd"));
        }

        [When(@"I remove one of the days from the work report")]
        public async Task WhenIRemoveOneOfTheDaysFromTheWorkReport()
        {
            var dayToRemove = _context.CurrentWorkReport!.Days.PickRandom();

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{_workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { dayToRemove }
                }, options: _context.JsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see that my work report contains one day less")]
        public async Task ThenICanSeeThatMyWorkReportContainsOneDayLess()
        {
            var tcs = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                while (true)
                {
                    tcs.Token.ThrowIfCancellationRequested();

                    using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports/{_workReportId}");

                    using var response = await httpClient.SendAsync(httpRequest, tcs.Token);
                    response.EnsureSuccessStatusCode();

                    try
                    {
                        var report = await response.Content.ReadFromJsonAsync<WorkReportDto>(_context.JsonSerializerOptions);
                        report!.Days.Should().HaveCount(_context.CurrentWorkReport!.Days.Count() - 1);
                        break;
                    }
                    catch (XunitException)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new XunitException("Unable to validate the step within the given delay");
            }
        }

        [When(@"I remove my work report")]
        public async Task WhenIRemoveMyWorkReport()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{_workReportId}");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [When(@"I send my report to the following recipients")]
        public async Task WhenISendMyReportToTheFollowingRecipients(Table table)
        {
            var emails = table.CreateSet(r => r["email"]);

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{_workReportId}/send")
            {
                Content = JsonContent.Create(new
                {
                    EmailAddresses = emails
                }, options: _context.JsonSerializerOptions)
            };

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

            var reports = await response.Content.ReadFromJsonAsync<WorkReportDto[]>(_context.JsonSerializerOptions);
            reports.Should().NotContainEquivalentOf(new
            {
                Id = _workReportId
            });
        }

        [Then(@"I can see that my work report has been sent to the following recipients")]
        public async Task ThenICanSeeThatMyWorkReportHasBeenSentToTheFollowingRecipients(Table table)
        {
            var emails = table.CreateSet(r => r["email"]);
            var tcs = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                while (true)
                {
                    tcs.Token.ThrowIfCancellationRequested();

                    using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports/{_workReportId}");

                    using var response = await httpClient.SendAsync(httpRequest, tcs.Token);
                    response.EnsureSuccessStatusCode();

                    try
                    {
                        var report = await response.Content.ReadFromJsonAsync<WorkReportDto>(_context.JsonSerializerOptions);
                        report!.Recipients.Should().BeEquivalentTo(emails);
                        break;
                    }
                    catch (XunitException)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new XunitException("Unable to validate the step within the given delay");
            }
        }

        [Then(@"I can see that the customer of my work report has been set")]
        public async Task ThenICanSeeThatTheCustomerOfMyWorkReportHasBeenSet()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"workreports/{_workReportId}");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var report = await response.Content.ReadFromJsonAsync<WorkReportDto>(_context.JsonSerializerOptions);
            report!.CustomerId.Should().Be(_context.CustomerId);
        }
    }
}
