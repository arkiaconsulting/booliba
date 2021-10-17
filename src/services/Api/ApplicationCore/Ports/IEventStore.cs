// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using System.Diagnostics;

namespace Booliba.ApplicationCore.Ports
{
    public interface IEventStore
    {
        Task Save(WorkReportEvent @event, CancellationToken cancellationToken = default);
        Task<IEnumerable<WorkReportEvent>> Load(Guid workReportId, CancellationToken cancellationToken = default);
    }
}
