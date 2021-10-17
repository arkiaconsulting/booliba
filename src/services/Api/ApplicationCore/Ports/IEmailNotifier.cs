// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.SendReport;

namespace Booliba.ApplicationCore.Ports
{
    public interface IEmailNotifier
    {
        Task Send(EmailMessage emailMessage, CancellationToken cancellationToken = default);
    }
}
