// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booliba.ApiTests.Fixture
{
    public class TestContext
    {
        public Uri ApiBaseUri => new(_host.Services.GetRequiredService<IConfiguration>()["API_BASE_URL"]);
        public ICollection<DateTimeOffset> CurrentWorkReport { get; set; } = new List<DateTimeOffset>();

        private readonly IHost _host;

        public TestContext()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                    builder.AddJsonFile("appsettings.Development.json", true).AddEnvironmentVariables())
                .Build();
        }
    }
}
