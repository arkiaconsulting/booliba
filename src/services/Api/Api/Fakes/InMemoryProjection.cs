// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Api.Fakes;
using Booliba.QuerySide;
using MediatR;
using System.Collections.Concurrent;
using System.Reflection;

namespace Booliba.Api.Fakes
{
    internal class InMemoryProjection : IWorkReportProjection
    {
        private readonly ConcurrentDictionary<Guid, WorkReportEntity> _workReportEntities = new();

        #region IWorkReportProjection

        Task<WorkReportEntity[]> IWorkReportProjection.List(CancellationToken cancellationToken) =>
            Task.FromResult(_workReportEntities.Values.ToArray());

        Task<WorkReportEntity?> IWorkReportProjection.Get(Guid workReportId, CancellationToken cancellationToken) =>
            Task.FromResult(
                _workReportEntities.GetValueOrDefault(workReportId)
            );
        Task IWorkReportProjection.Add(WorkReportEntity entity, CancellationToken cancellationToken)
        {
            _workReportEntities.AddOrUpdate(entity.Id, _ => entity, (_, _) => entity);

            return Task.CompletedTask;
        }

        Task IWorkReportProjection.Update(WorkReportEntity entity, CancellationToken cancellationToken)
        {
            _workReportEntities.AddOrUpdate(entity.Id, _ => entity, (_, _) => entity);

            return Task.CompletedTask;
        }

        Task IWorkReportProjection.Delete(Guid workReportId, CancellationToken cancellationToken)
        {
            _ = _workReportEntities.Remove(workReportId, out var _);

            return Task.CompletedTask;
        }

        #endregion
    }

    internal class InMemoryEventHandler :
        INotificationHandler<DaysAddedNotification>,
        INotificationHandler<ReportAddedNotification>,
        INotificationHandler<DaysRemovedNotification>,
        INotificationHandler<WorkReportRemovedNotification>,
        INotificationHandler<WorkReportSentNotification>
    {
        private readonly ProjectionService _projectionService;

        public InMemoryEventHandler(ProjectionService projectionService) => _projectionService = projectionService;

        Task INotificationHandler<DaysAddedNotification>.Handle(DaysAddedNotification notification, CancellationToken cancellationToken) =>
            _projectionService.AddDays(
                notification.Event.WorkReportId,
                notification.Event.Days.ToArray(),
                cancellationToken);

        Task INotificationHandler<ReportAddedNotification>.Handle(ReportAddedNotification notification, CancellationToken cancellationToken) =>
            _projectionService.CreateWorkReport(
                notification.Event.WorkReportId,
                notification.Event.WorkReportName,
                notification.Event.Days.ToArray(),
                cancellationToken);

        Task INotificationHandler<DaysRemovedNotification>.Handle(DaysRemovedNotification notification, CancellationToken cancellationToken) =>
            _projectionService.RemoveDays(
                notification.Event.WorkReportId,
                notification.Event.Days.ToArray(),
                cancellationToken);

        Task INotificationHandler<WorkReportRemovedNotification>.Handle(WorkReportRemovedNotification notification, CancellationToken cancellationToken) =>
            _projectionService.Remove(
                notification.Event.WorkReportId,
                cancellationToken);

        Task INotificationHandler<WorkReportSentNotification>.Handle(WorkReportSentNotification notification, CancellationToken cancellationToken) =>
            _projectionService.AddRecipients(
                notification.Event.WorkReportId,
                notification.Event.EmailAddresses,
                cancellationToken);
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    internal static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddInMemoryProjection(this IServiceCollection services) =>
            services.AddSingleton<IWorkReportProjection, InMemoryProjection>()
            .AddMediatR(Assembly.GetExecutingAssembly());
    }
}