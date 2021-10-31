// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Microsoft.Extensions.Logging;

namespace Booliba.QuerySide
{
    public class ProjectionService
    {
        private readonly IWorkReportProjection _workReportProjection;
        private readonly ILogger<ProjectionService> _logger;

        public ProjectionService(
            IWorkReportProjection workReportProjection,
            ILogger<ProjectionService> logger)
        {
            _workReportProjection = workReportProjection;
            _logger = logger;
        }

        public Task CreateWorkReport(Guid workReportId, string name, DateOnly[] days, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Creating work report {WorkReportName} {Days}", name, string.Join(" ", days.Select(d => d.ToString())));

            return _workReportProjection.Add(new WorkReportEntity(workReportId, name, days, Array.Empty<string>()), cancellationToken);
        }

        public async Task AddDays(Guid workReportId, DateOnly[] days, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Addind days to work report {Days}", string.Join(" ", days.Select(d => d.ToString())));

            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { Days = entity.Days.Concat(days).ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }

        public async Task RemoveDays(Guid workReportId, DateOnly[] daysToRemove, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Removing days from work report {Days}", string.Join(" ", daysToRemove.Select(d => d.ToString())));

            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { Days = entity.Days.Except(daysToRemove).ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }

        public Task Remove(Guid workReportId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Removing work report");

            return _workReportProjection.Delete(workReportId, cancellationToken);
        }

        public async Task AddRecipients(Guid workReportId, string[] recipientEmails, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding recipients to work report {EmailRecipients}", string.Join(" ", recipientEmails));

            var entity = await _workReportProjection.Get(workReportId, cancellationToken);
            var updatedEntity = entity! with { RecipientEmails = entity.RecipientEmails.Concat(recipientEmails).Distinct().ToArray() };

            await _workReportProjection.Update(updatedEntity, cancellationToken);
        }
    }
}
