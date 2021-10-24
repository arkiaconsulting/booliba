﻿// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.ApplicationCore.Ports
{
    public interface IEventStore
    {
        Task Save(IEnumerable<WorkReportEvent> events, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkReportEvent>> Load(Guid workReportId, CancellationToken cancellationToken = default);
    }
}
