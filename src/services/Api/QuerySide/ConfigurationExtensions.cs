// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Booliba.QuerySide
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddQuerySide(this IServiceCollection services) =>
            services.AddMediatR(Assembly.GetExecutingAssembly())
            .AddTransient<ProjectionService>();
    }
}
