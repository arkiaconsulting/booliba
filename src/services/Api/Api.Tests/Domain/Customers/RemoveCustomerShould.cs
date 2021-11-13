// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using AutoFixture;
using Booliba.ApplicationCore.Customers;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain.Customers
{
    [Trait("Category", "Unit")]
    public class RemoveCustomerShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public RemoveCustomerShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Pass")]
        [BoolibaInlineAutoData]
        public async Task Test01(RemoveCustomerCommand command) =>
            await _context.Sut.Send(command, CancellationToken.None);

        [Theory(DisplayName = "Effectively remove a customer")]
        [BoolibaInlineAutoData]
        public async Task Test02(RemoveCustomerCommand command, Fixture fixture)
        {
            _context.AddCustomer(command.CustomerId, fixture);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.CustomerId).OfType<CustomerRemoved>().Should().ContainSingle();
        }

        [Theory(DisplayName = "Pass when removing a customer that does not exist")]
        [BoolibaInlineAutoData]
        public async Task Test03(string customerName)
        {
            var command = new AddCustomerCommand(Guid.NewGuid(), customerName);

            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.CustomerId).OfType<CustomerRemoved>().Should().BeEmpty();
        }
    }
}
