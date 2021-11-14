// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Booliba.Data
{
    public class BoolibaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public BoolibaService(
            HttpClient httpClient,
            JsonSerializerOptions serializerOptions)
        {
            _httpClient = httpClient;
            _serializerOptions = serializerOptions;
        }

        internal async Task RemoveCustomer(Guid id)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"customers/{id}");

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task<WorkReport[]> GetWorkReports()
        {
            using var response = await _httpClient.GetAsync("workreports");
            response.EnsureSuccessStatusCode();

            var workReports = await response.Content.ReadFromJsonAsync<WorkReport[]>(_serializerOptions);

            return workReports!;
        }

        internal async Task<Customer[]> GetCustomers()
        {
            using var response = await _httpClient.GetAsync("customers");
            response.EnsureSuccessStatusCode();

            var customers = await response.Content.ReadFromJsonAsync<Customer[]>(_serializerOptions);

            return customers!;
        }

        internal async Task AddCustomer(Guid id, string name)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"customers/{id}")
            {
                Content = JsonContent.Create(new
                {
                    Name = name
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddDay(Guid workReportId, DateOnly day)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { day }
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddDays(Guid workReportId, DateOnly[] days)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = days
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveDay(Guid workReportId, DateOnly day)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { day }
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveDays(Guid workReportId, DateOnly[] days)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = days
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task Remove(Guid workReportId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"workreports/{workReportId}");

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task Send(Guid workReportId, IEnumerable<string> emails)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{workReportId}/send")
            {
                Content = JsonContent.Create(new
                {
                    EmailAddresses = emails
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateWorkReport(Guid workReportId, string name, IEnumerable<DateOnly> days)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{workReportId}")
            {
                Content = JsonContent.Create(new
                {
                    Name = name,
                    Days = days.ToArray()
                }, options: _serializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }
    }
}
