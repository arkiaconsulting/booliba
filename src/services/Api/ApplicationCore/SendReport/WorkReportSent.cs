// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;

namespace Booliba.ApplicationCore.SendReport
{
    public record WorkReportSent(Guid WorkReportId, string[] EmailAddresses) : WorkReportEvent(WorkReportId);
}
