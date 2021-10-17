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
using System.Linq;

namespace Booliba.Tests.Domain
{
    public sealed class TestContext
    {
        public IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        private ICollection<WorkReportEvent> _Events => (_host.Services.GetRequiredService<IEventStore>() as InMemoryEventStore)!.Events;

        private readonly IHost _host;

        public TestContext() =>
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddApplicationCore().AddInMemoryEventBus())
            .Build();

        public IEnumerable<WorkReportEvent> Events(Guid workReportId) =>
            _Events.Where(e => e.WorkReportId == workReportId);

        public (Guid Id, IEnumerable<DateOnly> Days) AddWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportAddedEvent = fixture.Build<ReportAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _Events.Add(workReportAddedEvent);

            return (workReportAddedEvent.WorkReportId, workReportAddedEvent.Days);
        }

        public (Guid Id, IEnumerable<DateOnly> Days) AddWorkReport(Fixture fixture)
        {
            var workReportAddedEvent = fixture.Create<ReportAdded>();

            _Events.Add(workReportAddedEvent);

            return (workReportAddedEvent.WorkReportId, workReportAddedEvent.Days);
        }

        public IEnumerable<DateOnly> AddDays(Guid workReportId, Fixture fixture)
        {
            var daysAdded = fixture.Build<DaysAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _Events.Add(daysAdded);

            return daysAdded.Days;
        }
    }
}
