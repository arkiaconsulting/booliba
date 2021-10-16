// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;

namespace Booliba.ApplicationCore.AddReport
{
    public record DaysAdded(Guid WorkReportId, IEnumerable<DateOnly> Days) : WorkReportEvent(WorkReportId);
}