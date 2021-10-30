// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

namespace Booliba.QuerySide
{
    public class ProjectionService
    {
        private readonly IWorkReportProjection _workReportProjection;

        public ProjectionService(IWorkReportProjection workReportProjection) =>
            _workReportProjection = workReportProjection;

        public Task CreateWorkReport(Guid workReportId, string name, DateOnly[] days, CancellationToken cancellationToken = default) =>
            _workReportProjection.Add(new WorkReportEntity(workReportId, name, days, Array.Empty<string>()), cancellationToken);

        public async Task AddDays(Guid workReportId, DateOnly[] days, CancellationToken cancellationToken = default)
        {
            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { Days = entity.Days.Concat(days).ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }

        public async Task RemoveDays(Guid workReportId, DateOnly[] daysToRemove, CancellationToken cancellationToken = default)
        {
            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { Days = entity.Days.Except(daysToRemove).ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }

        public async Task Remove(Guid workReportId, CancellationToken cancellationToken = default) =>
            await _workReportProjection.Delete(workReportId, cancellationToken);

        public async Task AddRecipients(Guid workReportId, string[] recipientEmails, CancellationToken cancellationToken = default)
        {
            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { RecipientEmails = entity.RecipientEmails.Concat(recipientEmails).Distinct().ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }
    }
}
