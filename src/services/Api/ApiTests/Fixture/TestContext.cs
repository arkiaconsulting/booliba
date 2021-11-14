// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Booliba.ApiTests.Fixture
{
    public class TestContext
    {
        public Uri ApiBaseUri => new(_host.Services.GetRequiredService<IConfiguration>()["API_BASE_URL"]);
        public TestWorkReport? CurrentWorkReport { get; set; }
        public JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;

        public Guid? CustomerId { get; private set; }

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHost _host;

        public TestContext()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                builder.AddJsonFile("appsettings.Development.json", true).AddEnvironmentVariables())
                .Build();
            _jsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        }

        internal void SetCustomer(Guid customerId) => CustomerId = customerId;
    }

    public record TestWorkReport(string Name, IEnumerable<DateOnly> Days, Guid? CustomerId);
}
