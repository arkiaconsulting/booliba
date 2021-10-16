// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services) =>
            services.AddSingleton<IEventBus, InMemoryEventBus>();
    }
}
