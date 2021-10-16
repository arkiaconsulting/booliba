// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Tests.Fixtures;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests
{
    [Trait("Category", "Component")]
    public class CosmosReportTests : IAsyncLifetime
    {
        private CosmosClient CosmosClient => _host.Services.GetRequiredService<CosmosClient>();
        private Container ReportsContainer => _container ?? throw new InvalidOperationException("Wrong initialization");

        private readonly IHost _host;
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private Container? _container;

        public CosmosReportTests() => _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddSingleton(CosmosFixture.EmulatorClient))
                .Build();

        [Fact]
        public async Task Test1()
        {

        }

        #region IAsyncLifetime

        async Task IAsyncLifetime.InitializeAsync()
        {
            Database db = await CosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName, 10000);
            _container = await CosmosFixture.CreateReportsContainer(db);
        }

        async Task IAsyncLifetime.DisposeAsync() => await CosmosClient.GetDatabase(_databaseName).DeleteAsync();

        #endregion
    }
}