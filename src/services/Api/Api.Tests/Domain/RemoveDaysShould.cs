// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.AddDaysToReport;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Booliba.ApplicationCore.RemoveDaysFromReport;

namespace Booliba.Tests.Domain
{
    [Trait("Category","Unit")]
    public class RemoveDaysShould
    {
        private IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        private ICollection<DomainEvent> Events => (_host.Services.GetRequiredService<IEventBus>() as InMemoryEventBus)!.Events;

        private readonly IHost _host;

        public RemoveDaysShould() => _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddApplicationCore().AddInMemoryEventBus())
                .Build();

        [Theory(DisplayName = "Effectively remove a day from the report"), BoolibaInlineAutoData]
        public async Task Test01(Fixture fixture)
        {
            var reportAddedEvent = AddWorkReport(fixture);
            var dayToRemove = reportAddedEvent.Days.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, reportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { dayToRemove })
                .Create();

            await Sut.Send(removeDaysCommand, CancellationToken.None);

            Events.OfType<DaysRemoved>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(dayToRemove);
        }

        [Theory(DisplayName = "Not remove a day that was not in the report"), BoolibaInlineAutoData]
        public async Task Test02(Fixture fixture)
        {
            var workReportAddedEvent = AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { fixture.Create<DateOnly>() })
                .Create();

            await Sut.Send(addDaysCommand, CancellationToken.None);

            Events.OfType<DaysRemoved>().Should().BeEmpty();
        }

        [Theory(DisplayName = "Remove a day that was previously added"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var reportAddedEvent = AddWorkReport(fixture);
            var daysAddedEvent = AddDays(reportAddedEvent.WorkReportId, fixture);

            var dayToRemove = daysAddedEvent.Days.PickRandom();
            var removeDaysCommand = fixture.Build<RemoveDaysCommand>()
                .With(c => c.WorkReportId, reportAddedEvent.WorkReportId)
                .With(c => c.DaysToRemove, new[] { dayToRemove })
                .Create();

            await Sut.Send(removeDaysCommand, CancellationToken.None);

            Events.OfType<DaysRemoved>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(dayToRemove);
        }

        #region Private

        private DaysAdded AddDays(Guid workReportId, Fixture fixture)
        {
            var daysAdded = fixture.Build<DaysAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            Events.Add(daysAdded);

            return daysAdded;
        }

        private ReportAdded AddWorkReport(Fixture fixture)
        {
            var workReportAddedEvent = fixture.Create<ReportAdded>();

            Events.Add(workReportAddedEvent);

            return workReportAddedEvent;
        }

        #endregion
    }
}
