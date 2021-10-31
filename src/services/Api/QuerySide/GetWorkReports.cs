// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;
using Microsoft.Extensions.Logging;

namespace Booliba.QuerySide
{
    public record GetWorkReportsQuery : IRequest<GetWorkReportsResponse>;
    public record GetWorkReportsResponse(WorkReportEntity[] Results);
    public record WorkReportEntity(Guid Id, string Name, DateOnly[] Days, string[] RecipientEmails);

    internal class GetWorkReportsQueryHandler : IRequestHandler<GetWorkReportsQuery, GetWorkReportsResponse>
    {
        private readonly IWorkReportProjection _workReportProjection;
        private readonly ILogger<GetWorkReportsQueryHandler> _logger;

        public GetWorkReportsQueryHandler(
            IWorkReportProjection workReportProjection,
            ILogger<GetWorkReportsQueryHandler> logger)
        {
            _workReportProjection = workReportProjection;
            _logger = logger;
        }

        async Task<GetWorkReportsResponse> IRequestHandler<GetWorkReportsQuery, GetWorkReportsResponse>.Handle(GetWorkReportsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling a query {QueryType}", request.GetType().Name);

            var workReports = await _workReportProjection.List(cancellationToken);

            return new GetWorkReportsResponse(workReports);
        }
    }
}
