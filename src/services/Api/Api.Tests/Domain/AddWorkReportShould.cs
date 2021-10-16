// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture.Xunit2;
using Booliba.ApplicationCore.AddReport;
using Booliba.ApplicationCore.Ports;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System;
using AutoFixture;

namespace Booliba.Tests.Domain
{
    [Trait("Category","Unit")]
    public class AddWorkReportShould
    {
        private IMediator Sut => _host.Services.GetRequiredService<IMediator>();
        private IEnumerable<DomainEvent> Events => (_host.Services.GetRequiredService<IEventBus>() as InMemoryEventBus)!.Events;
        
        private readonly IHost _host;

        public AddWorkReportShould() => _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddApplicationCore().AddInMemoryEventBus())
                .Build();

        [Theory(DisplayName = "Pass"),BoolibaInlineAutoData]
        public async Task Test01(AddWorkReportCommand command) => await Sut.Send(command, CancellationToken.None);

        [Theory(DisplayName = "Produce an event"), BoolibaInlineAutoData]
        public async Task Test02(AddWorkReportCommand command)
        {
            await Sut.Send(command, CancellationToken.None);

            Events.Should().ContainSingle();
        }

        [Theory(DisplayName = "Handle setting the work days"), BoolibaInlineAutoData]
        public async Task Test03(AddWorkReportCommand command)
        {
            await Sut.Send(command, CancellationToken.None);

            Events.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    command.Days
                });
        }

        [Theory(DisplayName = "Uniquely identify a work report"), BoolibaInlineAutoData]
        public async Task Test04(AddWorkReportCommand command)
        {
            await Sut.Send(command, CancellationToken.None);

            Events.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    WorkReportId = command.Id
                });
        }

        [Fact(DisplayName = "Remove day duplicates")]
        public async Task Test05()
        {
            var command = new AddWorkReportCommand(Guid.NewGuid(), Guid.NewGuid().ToString(), new[]
            {
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now)
            });

            await Sut.Send(command, CancellationToken.None);

            Events.Should().ContainSingle()
                .Which.Should().BeOfType<ReportAdded>()
                .Which.Days.Should().ContainSingle();
        }

        [Theory(DisplayName = "Handle setting the work report name"), BoolibaInlineAutoData]
        public async Task Test06(AddWorkReportCommand command)
        {
            await Sut.Send(command, CancellationToken.None);

            Events.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    WorkReportName = command.Name
                });
        }
    }
}
