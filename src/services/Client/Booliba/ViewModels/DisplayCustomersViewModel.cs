// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using System;
using System.Threading.Tasks;

namespace Booliba.ViewModels
{
    internal class DisplayCustomersViewModel
    {
        public Guid Id { get; }
        public string Name { get; }

        private readonly BoolibaService _service;

        public DisplayCustomersViewModel(
            BoolibaService service,
            Guid id,
            string name)
        {
            _service = service;
            Id = id;
            Name = name;
        }

        internal async Task Remove() => await _service.RemoveCustomer(Id);
    }
}
