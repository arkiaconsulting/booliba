﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;

namespace Booliba.ApplicationCore.RemoveDaysFromReport
{
    public record DaysRemoved(Guid AggregateId, IEnumerable<DateOnly> Days) : DomainEvent(AggregateId);
}
