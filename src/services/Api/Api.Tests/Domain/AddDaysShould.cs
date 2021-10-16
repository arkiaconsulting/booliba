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

namespace Booliba.Tests.Domain
{
    [Trait("Category","Unit")]
    public class AddDaysShould
    {
        private IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        private ICollection<DomainEvent> Events => (_host.Services.GetRequiredService<IEventBus>() as InMemoryEventBus)!.Events;

        private readonly IHost _host;

        public AddDaysShould() => _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddApplicationCore().AddInMemoryEventBus())
                .Build();

        [Theory(DisplayName = "Produce the right event"), BoolibaInlineAutoData]
        public async Task Test01(AddDaysCommand command, Fixture fixture)
        {
            AddWorkReport(command.WorkReportId, fixture);

            await Sut.Send(command, CancellationToken.None);

            Events.OfType<DaysAdded>().Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Days = command.DaysToAdd
                });
        }

        [Theory(DisplayName = "Not add the days if they already are in the report"), BoolibaInlineAutoData]
        public async Task Test02(Fixture fixture)
        {
            var workReportAddedEvent = AddWorkReport(fixture);
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToAdd, new[] { workReportAddedEvent.Days.PickRandom() })
                .Create();

            await Sut.Send(addDaysCommand, CancellationToken.None);

            Events.Should().ContainSingle();
        }

        [Theory(DisplayName = "Not add the days that already are in the report"), BoolibaInlineAutoData]
        public async Task Test03(Fixture fixture)
        {
            var workReportAddedEvent = AddWorkReport(fixture);
            var effectivelyNewDay = fixture.Create<DateOnly>();
            var addDaysCommand = fixture.Build<AddDaysCommand>()
                .With(c => c.WorkReportId, workReportAddedEvent.WorkReportId)
                .With(c => c.DaysToAdd, new[] { workReportAddedEvent.Days.PickRandom(), effectivelyNewDay })
                .Create();

            await Sut.Send(addDaysCommand, CancellationToken.None);

            Events.OfType<DaysAdded>().Should().ContainSingle()
                .Which.Days.Should().ContainSingle()
                .Which.Should().Be(effectivelyNewDay);
        }

        #region Private

        private ReportAdded AddWorkReport(Guid workReportId, Fixture fixture)
        {
            var workReportAddedEvent = fixture.Build<ReportAdded>()
                .With(c => c.WorkReportId, workReportId)
                .Create();

            Events.Add(workReportAddedEvent);

            return workReportAddedEvent;
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
