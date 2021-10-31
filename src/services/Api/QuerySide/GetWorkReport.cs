// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;
using Microsoft.Extensions.Logging;

namespace Booliba.QuerySide
{
    public record GetWorkReportQuery(Guid WorkReportId) : IRequest<GetWorkReportResponse>;
    public record GetWorkReportResponse(WorkReportEntity? Result);

    internal class GetWorkReportQueryHandler : IRequestHandler<GetWorkReportQuery, GetWorkReportResponse>
    {
        private readonly IWorkReportProjection _workReportProjection;
        private readonly ILogger<GetWorkReportQueryHandler> _logger;

        public GetWorkReportQueryHandler(
            IWorkReportProjection workReportProjection,
            ILogger<GetWorkReportQueryHandler> logger)
        {
            _workReportProjection = workReportProjection;
            _logger = logger;
        }

        async Task<GetWorkReportResponse> IRequestHandler<GetWorkReportQuery, GetWorkReportResponse>.Handle(GetWorkReportQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling a query {QueryType}", request.GetType().Name);

            var workReports = await _workReportProjection.Get(request.WorkReportId, cancellationToken);

            return new GetWorkReportResponse(workReports);
        }
    }
}
