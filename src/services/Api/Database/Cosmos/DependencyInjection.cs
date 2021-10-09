// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Azure.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp =>
            {
                var credential = sp.GetRequiredService<TokenCredential>();

                return new CosmosClient(configuration["Cosmos:Endpoint"], credential);
            });

            return services;
        }
    }
}
