// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Customers;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain.Customers
{
    [Trait("Category", "Unit")]
    public class AddCustomerShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public AddCustomerShould(TestContext context) => _context = context;

        [Theory(DisplayName = "Pass")]
        [BoolibaInlineAutoData]
        public async Task Test01(AddCustomerCommand command) =>
            await _context.Sut.Send(command, CancellationToken.None);

        [Theory(DisplayName = "Effectively add a customer")]
        [BoolibaInlineAutoData]
        public async Task Test02(AddCustomerCommand command)
        {
            await _context.Sut.Send(command, CancellationToken.None);

            _context.Events(command.CustomerId).OfType<CustomerAdded>().Should().ContainSingle();
        }
    }
}
