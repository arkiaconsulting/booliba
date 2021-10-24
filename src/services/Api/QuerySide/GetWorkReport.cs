// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using MediatR;

namespace Booliba.QuerySide
{
    public record GetWorkReportQuery(Guid WorkReportId) : IRequest<GetWorkReportResponse>;
    public record GetWorkReportResponse(WorkReportEntity? Result);

    internal class GetWorkReportQueryHandler : IRequestHandler<GetWorkReportQuery, GetWorkReportResponse>
    {
        private readonly IWorkReportProjection _workReportProjection;

        public GetWorkReportQueryHandler(IWorkReportProjection workReportProjection) =>
            _workReportProjection = workReportProjection;

        async Task<GetWorkReportResponse> IRequestHandler<GetWorkReportQuery, GetWorkReportResponse>.Handle(GetWorkReportQuery request, CancellationToken cancellationToken)
        {
            var workReports = await _workReportProjection.Get(request.WorkReportId, cancellationToken);

            return new GetWorkReportResponse(workReports);
        }
    }
}
