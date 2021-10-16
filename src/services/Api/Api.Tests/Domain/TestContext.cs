// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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

        public (Guid Id, IEnumerable<DateOnly> Days) AddWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportAddedEvent = fixture.Build<ReportAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            Events.Add(workReportAddedEvent);

            return (workReportAddedEvent.WorkReportId, workReportAddedEvent.Days);
        }

        public (Guid Id, IEnumerable<DateOnly> Days) AddWorkReport(Fixture fixture)
        {
            var workReportAddedEvent = fixture.Create<ReportAdded>();

            Events.Add(workReportAddedEvent);

            return (workReportAddedEvent.WorkReportId, workReportAddedEvent.Days);
        }

        public IEnumerable<DateOnly> AddDays(Guid workReportId, Fixture fixture)
        {
            var daysAdded = fixture.Build<DaysAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            Events.Add(daysAdded);

            return daysAdded.Days;
        }
    }
}
