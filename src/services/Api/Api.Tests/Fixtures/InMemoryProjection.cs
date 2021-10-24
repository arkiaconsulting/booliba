// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.QuerySide;
using Booliba.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booliba.Tests.Fixtures
{
    internal class InMemoryProjection : IWorkReportProjection
    {
        public ICollection<WorkReportEntity> WorkReports => _workReportEntities;

        private readonly ICollection<WorkReportEntity> _workReportEntities = new List<WorkReportEntity>();

        Task<WorkReportEntity[]> IWorkReportProjection.List(CancellationToken cancellationToken) =>
            Task.FromResult(_workReportEntities.ToArray());

        Task<WorkReportEntity?> IWorkReportProjection.Get(Guid workReportId, CancellationToken cancellationToken) =>
            Task.FromResult(
                _workReportEntities
                .SingleOrDefault(wr => wr.Id == workReportId)
            );
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryProjection(this IServiceCollection services) =>
            services.AddSingleton<IWorkReportProjection, InMemoryProjection>();
    }
}