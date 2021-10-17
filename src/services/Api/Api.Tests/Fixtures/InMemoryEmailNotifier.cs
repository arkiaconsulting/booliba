// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.ApplicationCore.Ports;
using Booliba.ApplicationCore.SendReport;
using Booliba.Tests.Fixtures;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Booliba.Tests.Fixtures
{
    internal class InMemoryEmailNotifier : IEmailNotifier
    {
        public IEnumerable<EmailMessage> Emails => _emails;

        private readonly ICollection<EmailMessage> _emails = new HashSet<EmailMessage>();

        Task IEmailNotifier.Send(EmailMessage emailMessage, CancellationToken cancellationToken)
        {
            _emails.Add(emailMessage);

            return Task.CompletedTask;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryEmailNotifier(this IServiceCollection services) =>
            services.AddSingleton<IEmailNotifier, InMemoryEmailNotifier>();
    }
}
