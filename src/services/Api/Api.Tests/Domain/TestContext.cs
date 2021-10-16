// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Booliba.Tests.Domain
{
    public class TestContext
    {
        public IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        public ICollection<DomainEvent> Events => (_host.Services.GetRequiredService<IEventBus>() as InMemoryEventBus)!.Events;

        private readonly IHost _host;

        public TestContext() =>
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddApplicationCore().AddInMemoryEventBus())
            .Build();
    }
}
