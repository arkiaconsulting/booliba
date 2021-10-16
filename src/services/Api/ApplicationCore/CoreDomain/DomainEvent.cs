// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.ApplicationCore.Ports
{
    public abstract record DomainEvent();

    public abstract record WorkReportEvent(Guid WorkReportId) : DomainEvent;
}