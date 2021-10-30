﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using Booliba.ApplicationCore.RemoveDaysFromReport;
using Booliba.ApplicationCore.RemoveWorkReport;
using Booliba.ApplicationCore.SendReport;
using Booliba.QuerySide;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Booliba.Tests.Fixtures
{
    public sealed class TestContext
    {
        public IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        public IEnumerable<WorkReportEntity> Entities => _Entities;

        private ICollection<WorkReportEvent> _Events => (_host.Services.GetRequiredService<IEventStore>() as InMemoryEventStore)!.Events;
        private IEnumerable<EmailMessage> _Emails => (_host.Services.GetRequiredService<IEmailNotifier>() as InMemoryEmailNotifier)!.Emails;
        public ProjectionService ProjectionService => _host.Services.GetRequiredService<ProjectionService>();
        private ICollection<WorkReportEntity> _Entities => (_host.Services.GetRequiredService<IWorkReportProjection>() as InMemoryProjection)!.WorkReports;

        private readonly IHost _host;

        public TestContext() =>
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
                services.AddApplicationCore()
                .AddInMemoryEventBus()
                .AddInMemoryEmailNotifier()
                .AddQuerySide().AddInMemoryProjection()
            ).Build();

        public IEnumerable<WorkReportEvent> Events(Guid workReportId) =>
            _Events.Where(e => e.WorkReportId == workReportId);

        public IEnumerable<EmailMessage> Emails(Guid workReportId) =>
            _Emails.Where(e => e.WorkReportId == workReportId);

        public (Guid Id, IEnumerable<DateOnly> Days) AddWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportAddedEvent = fixture.Build<ReportAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _Events.Add(workReportAddedEvent);

            return (workReportAddedEvent.WorkReportId, workReportAddedEvent.Days);
        }

        internal void RemoveWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportRemovedEvent = fixture.Build<WorkReportRemoved>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            _Events.Add(workReportRemovedEvent);
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

        internal IEnumerable<DateOnly> RemoveDays(Guid workReportId, IEnumerable<DateOnly> daysToRemove, Fixture fixture)
        {
            var daysRemovedEvent = fixture.Build<DaysRemoved>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.Days, daysToRemove)
                .Create();

            _Events.Add(daysRemovedEvent);

            return daysRemovedEvent.Days;
        }

        internal void Send(Guid workReportId, string[] emailAddresses, Fixture fixture)
        {
            var sentEvent = fixture.Build<WorkReportSent>()
                .With(c => c.WorkReportId, workReportId)
                .With(c => c.EmailAddresses, emailAddresses)
                .Create();

            _Events.Add(sentEvent);
        }

        internal void AddProjectedWorkReport(Guid workReportId, string workReportName, DateOnly[] days, string[]? recipientEmails = default) =>
            _Entities.Add(new WorkReportEntity(workReportId, workReportName, days, recipientEmails ?? Array.Empty<string>()));
    }
}
