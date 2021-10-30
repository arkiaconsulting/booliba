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

        #region IWorkReportProjection

        Task<WorkReportEntity[]> IWorkReportProjection.List(CancellationToken cancellationToken) =>
            Task.FromResult(_workReportEntities.ToArray());

        Task<WorkReportEntity?> IWorkReportProjection.Get(Guid workReportId, CancellationToken cancellationToken) =>
            Task.FromResult(
                _workReportEntities
                .SingleOrDefault(wr => wr.Id == workReportId)
            );
        Task IWorkReportProjection.Add(WorkReportEntity entity, CancellationToken cancellationToken)
        {
            _workReportEntities.Add(entity);

            return Task.CompletedTask;
        }

        Task IWorkReportProjection.Update(WorkReportEntity entity, CancellationToken cancellationToken)
        {
            var existingEntity = _workReportEntities.Single(e => e.Id == entity.Id);
            _workReportEntities.Remove(existingEntity);
            _workReportEntities.Add(entity);

            return Task.CompletedTask;
        }

        Task IWorkReportProjection.Delete(Guid workReportId, CancellationToken cancellationToken)
        {
            var existingEntity = _workReportEntities.Single(e => e.Id == workReportId);
            _workReportEntities.Remove(existingEntity);

            return Task.CompletedTask;
        }

        #endregion
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