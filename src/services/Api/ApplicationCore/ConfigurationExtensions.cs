﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplicationCore(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
