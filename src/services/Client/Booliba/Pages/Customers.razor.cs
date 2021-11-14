// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using Booliba.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booliba.Pages
{
    public partial class Customers
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private BoolibaService BoolibaService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private List<DisplayCustomersViewModel>? _customers = null;
        private string _newCustomerName = string.Empty;
        protected override async Task OnInitializedAsync() =>
            _customers = (await BoolibaService.GetCustomers())
            .Select(wr => new DisplayCustomersViewModel(BoolibaService, wr.Id, wr.Name))
            .ToList();

        private async Task AddCustomer()
        {
            var customerId = Guid.NewGuid();
            await BoolibaService.AddCustomer(customerId, _newCustomerName);
            _customers!.Insert(0, new(BoolibaService, customerId, _newCustomerName));
        }

        private async Task Remove(DisplayCustomersViewModel customer)
        {
            await customer.Remove();

            _customers!.Remove(customer);
        }
    }
}
