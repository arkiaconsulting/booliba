// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System;
using System.Threading.Tasks;

namespace Booliba.Tests.Fixtures
{
    internal class CosmosFixture : IDisposable
    {
        private static readonly CosmosClient CosmosClient;

        public static CosmosClient EmulatorClient => CosmosClient;

        static CosmosFixture() => CosmosClient = new CosmosClientBuilder(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==")
            .WithSerializerOptions(new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
            .Build();

        public void Dispose() => ((IDisposable)CosmosClient).Dispose();

        internal static async Task<Container> CreateReportsContainer(Database database) => (await database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = "reports",
            PartitionKeyPath = "/userId"
        })).Container;
    }
}
