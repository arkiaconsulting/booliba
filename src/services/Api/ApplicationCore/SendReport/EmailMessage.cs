// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.ApplicationCore.SendReport
{
    public record EmailMessage(Guid WorkReportId, string[] EmailAddresses);
}
