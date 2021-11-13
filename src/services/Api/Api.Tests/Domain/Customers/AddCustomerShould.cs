// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Booliba.Tests.Domain.Customers
{
    [Trait("Category", "Unit")]
    public class AddCustomerShould : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public AddCustomerShould(TestContext context)
        {
            _context = context;
        }
    }
}
