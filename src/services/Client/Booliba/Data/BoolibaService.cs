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
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly HttpClient _httpClient;

        public BoolibaService(HttpClient httpClient)
        {
            JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
            _httpClient = httpClient;
        }

        public async Task<WorkReport[]> GetWorkReports()
        {
            using var response = await _httpClient.GetAsync("workreports");
            response.EnsureSuccessStatusCode();

            var workReports = await response.Content.ReadFromJsonAsync<WorkReport[]>(JsonSerializerOptions);

            return workReports!;
        }

        public async Task AddDay(Guid workReportId, DateOnly day)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"workreports/{workReportId}/days")
            {
                Content = JsonContent.Create(new
                {
                    Days = new[] { day }
                }, options: JsonSerializerOptions)
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
                }, options: JsonSerializerOptions)
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
                }, options: JsonSerializerOptions)
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
                }, options: JsonSerializerOptions)
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
                }, options: JsonSerializerOptions)
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
                }, options: JsonSerializerOptions)
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }
    }
}
