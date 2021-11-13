// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.QuerySide;
using Booliba.Tests.Fixtures;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Query
{
    [Trait("Category", "Unit")]
    public class CustomerProjectionServiceShould
    {
        private readonly TestContext _context;

        public CustomerProjectionServiceShould() => _context = new TestContext();

        [Theory(DisplayName = "Project a new customer"), BoolibaInlineAutoData]
        public async Task Test01(Guid customerId, string name)
        {
            await _context.ProjectionService.CreateCustomer(customerId, name);

            _context.CustomerEntities.Should().ContainEquivalentOf(
                new CustomerEntity(customerId, name)
            );
        }

        [Theory(DisplayName = "Project when an existing customer is removed"), BoolibaInlineAutoData]
        public async Task Test04(Guid customerId, string name)
        {
            _context.AddProjectedCustomer(customerId, name);

            await _context.ProjectionService.RemoveCustomer(customerId);

            _context.CustomerEntities.Should().NotContainEquivalentOf(new
            {
                Id = customerId
            });
        }
    }
}
