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
    internal class InMemoryProjection : IWorkReportProjection, ICustomerProjection
    {
        public ICollection<WorkReportEntity> WorkReports => _workReportEntities;
        public ICollection<CustomerEntity> Customers => _customerEntities;

        private readonly ICollection<WorkReportEntity> _workReportEntities = new List<WorkReportEntity>();
        private readonly ICollection<CustomerEntity> _customerEntities = new List<CustomerEntity>();

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

        #region ICustomerProjection

        Task ICustomerProjection.Add(CustomerEntity entity, CancellationToken cancellationToken)
        {
            _customerEntities.Add(entity);

            return Task.CompletedTask;
        }

        Task ICustomerProjection.Delete(Guid customerId, CancellationToken cancellationToken)
        {
            var existingEntity = _customerEntities.Single(e => e.Id == customerId);
            _customerEntities.Remove(existingEntity);

            return Task.CompletedTask;
        }

        Task<CustomerEntity[]> ICustomerProjection.List(CancellationToken cancellationToken) =>
            Task.FromResult(_customerEntities.ToArray());

        #endregion
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryProjection(this IServiceCollection services) =>
            services.AddSingleton<IWorkReportProjection, InMemoryProjection>()
            .AddSingleton<ICustomerProjection, InMemoryProjection>();
    }
}