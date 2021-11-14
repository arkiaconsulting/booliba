// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApiTests.Fixture;
using FluentAssertions;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Booliba.ApiTests
{
    [Binding]
    public class CustomersStepDefinitions
    {
        private readonly TestContext _context;
        private string? _newCustomerName;
        private Guid? _customerId;

        public CustomersStepDefinitions(TestContext context) => _context = context;

        [Given(@"I had a contract with a new customer named '([^']*)'")]
        public void GivenIHadAContractWithANewCustomerNamed(string customerName) =>
            _newCustomerName = customerName;

        [Given(@"I have added a customer named '([^']*)'")]
        public async Task GivenIHaveAddedACustomerNamedAsync(string contoso)
        {
            _customerId = Guid.NewGuid();

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"customers/{_customerId}")
            {
                Content = JsonContent.Create(new
                {
                    Name = contoso
                },
                options: _context.JsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            _context.SetCustomer(_customerId!.Value);
        }

        [When(@"I add a new customer")]
        public async Task WhenIAddANewCustomerAsync()
        {
            _customerId = Guid.NewGuid();

            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"customers/{_customerId}")
            {
                Content = JsonContent.Create(new
                {
                    Name = _newCustomerName
                },
                options: _context.JsonSerializerOptions)
            };

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        [Then(@"I can see my new customer in the customers list")]
        public async Task ThenICanSeeMyNewCustomerInTheCustomersListAsync()
        {
            using var httpClient = new HttpClient { BaseAddress = _context.ApiBaseUri };
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "customers");

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var customers = await response.Content.ReadFromJsonAsync<WorkReportDto[]>(_context.JsonSerializerOptions);
            customers.Should().ContainEquivalentOf(new
            {
                Id = _customerId,
                Name = _newCustomerName
            });
        }
    }
}
