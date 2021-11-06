// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System;
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

        public BoolibaService() => JsonSerializerOptions.Converters.Add(new DateOnlyConverter());

        public async Task<WorkReport[]> GetWorkReports()
        {
            using var httpClient = new HttpClient { BaseAddress = new Uri("https://booliba.azurewebsites.net/api/") };
            using var response = await httpClient.GetAsync("workreports");
            response.EnsureSuccessStatusCode();

            var workReports = await response.Content.ReadFromJsonAsync<WorkReport[]>(JsonSerializerOptions);

            return workReports!;
        }
    }
}
